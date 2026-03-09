/*
    Auto Layout

    Automatically lays out planar parts for machining using a binary tree packing method.

            1.0     - May  6, 2016 - Marena Richardson - Initial Demo Version published to forums.
            2.0     - Mar 20, 2018 - Arul Suresh       - Updated to work in-context, multiple features.
            2.0.1   - Mar 20, 2018 - Arul Suresh       - Removed blank feature definition.
            2.1     - Mar 22, 2018 - Arul Suresh       - Added info sheet and posted on OS forum.
            2.1.1   - Mar 22, 2018 - Arul Suresh       - Fixed bug with more than two Auto Layout features in one Part Studio.
            2.2     - May 15, 2018 - Arul Suresh       - Fixed bug with finding rotated placements.
*/

FeatureScript 819;
import(path : "onshape/std/geometry.fs", version : "819.0");
import(path : "58d5a077de110acea0161534", version : "0e4e937a643cb5e5239d28e2"); // auto_layout/auto_layout-config.fs
import(path : "541cf3a49160fa696f01a868", version : "1e7a0cca03505753a865fd08"); // auto_layout/auto_layout-types.fs
import(path : "f4e7238da5afaf5a3f1498c0/1401405144d557c6277a5b97/22d17eb94c85900576fbf53e", version : "5ef14ff6ba935dae98b7a671"); // laser_utils/laser_utils.fs

annotation { "Feature Type Name" : "Auto Layout" }
export const autolayout = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Thickness of material", "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
        isLength(definition.thickness, NONNEGATIVE_LENGTH_BOUNDS);

        annotation { "Name" : "Cut sheet width", "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
        isLength(definition.width, DEFAULT_SHEET_WIDTH);

        annotation { "Name" : "Cut sheet height", "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
        isLength(definition.height, DEFAULT_SHEET_HEIGHT);

        annotation { "Name" : "Spacing", "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
        isLength(definition.spacing, DEFAULT_SPACING);

        annotation { "Name" : "Multiple Copies", "UIHint" : "DISPLAY_SHORT" }
        definition.copies is boolean;

        if (definition.copies)
        {
            annotation { "Name" : "Number of copies", "UIHint" : "DISPLAY_SHORT" }
            isInteger(definition.N, POSITIVE_COUNT_BOUNDS);
        }

        annotation { "Name" : "Show Cut Sheet Sketches", "UIHint" : "REMEMBER_PREVIOUS_VALUE" }
        definition.showSheets is boolean;

        annotation { "Name" : "Test Part", "Filter" : EntityType.BODY, "MaxNumberOfPicks" : 1 }
        definition.testP is Query;

    }
    {
        //Check for previous Auto Layouts
        var initialY = 0 * meter;
        try silent
        {
            initialY = getVariable(context, "AutoLayout_yinitial");
        }

        // Query for all bodies
        var bodies = qAllModifiableSolidBodies();

        // Filter out those placed by a previous AutoLayout feature
        var hasAttribute = qAttributeQuery("" as AutoLayoutAttribute);
        bodies = qSubtraction(bodies, hasAttribute);

        // Select parts having the right thickness
        var operBodies = qFilterFunction(bodies, context, function(body)
        {
            return tolerantEquals(getBoundingThickness(context, body), definition.thickness);
        });

        if (evaluateQuery(context, operBodies) == [])
        {
            throw regenError("Could not find parts with given thickness", ["thickness"]);
        }

        // Pattern if nesting multiple copies
        if (definition.copies && definition.N > 1)
        {
            var M = definition.N - 1;
            var transformArray = makeArray(M, identityTransform());
            var instanceArray = makeArray(M, "");
            for (var i = 0; i < M; i += 1)
            {
                instanceArray[i] = "" ~ i;
            }
            opPattern(context, id + "make_copies", {
                        "entities" : operBodies,
                        "transforms" : transformArray,
                        "instanceNames" : instanceArray
                    });
            operBodies = qUnion([operBodies, qCreatedBy(id + "make_copies", EntityType.BODY)]);
        }

        // Initialize list of parameters necessary for bin packing
        var blocks = [];

        // Iterate through bodies to layout in this operation
        var N = size(evaluateQuery(context, operBodies));
        for (var i = 0; i < N; i += 1)
        {
            // Get largest planar face
            var face = getLargestFace(context, qNthElement(operBodies, i));

            // If part has planar faces
            if (evaluateQuery(context, face) != [])
            {
                // Find bounding box with normal defined by the largest planar face
                var largestFacePlane = evPlane(context, {
                        "face" : face
                    });
                const orientedCSys = planeToCSys(largestFacePlane);
                const bbox is Box3d = evBox3d(context, {
                            "topology" : qNthElement(operBodies, i),
                            "cSys" : orientedCSys
                        });
                const deltaX = abs(bbox.maxCorner[0] - bbox.minCorner[0]);
                const deltaY = abs(bbox.maxCorner[1] - bbox.minCorner[1]);
                const deltaZ = abs(bbox.maxCorner[2] - bbox.minCorner[2]);

                // Calculate transform to the top plane and to the origin
                var transformFromWorld = fromWorld(orientedCSys);
                var transformToOrigin = transform(-bbox.minCorner);

                //DEBUG
                //var dbg = (size(evaluateQuery(context, qUnion([qNthElement(operBodies, i), definition.testP]))) == 1);

                blocks = append(blocks, new box({ 'w' : deltaX, 'h' : deltaY, 'owner' : qNthElement(operBodies, i), 'transform' : transformToOrigin * transformFromWorld, 'rotated' : false })); //, 'debug' : dbg }));
            }
        }

        // Move non laser cuttable parts out of the way
        var noParts = qSubtraction(bodies, operBodies);
        if (evaluateQuery(context, noParts) != [])
        {
            const bbox is Box3d = evBox3d(context, {
                        "topology" : noParts
                    });

            var transformToOrigin = transform(-bbox.maxCorner);
            var transformAway = transform(vector(-(definition.width * 0.3), 0 * meter, 0 * meter));

            opTransform(context, id + "transform_noParts", {
                        "bodies" : noParts,
                        "transform" : transformAway * transformToOrigin
                    });
        }


        // Initialize lists for determining layout
        var sortedBlocks = sortBlocks(blocks);
        var prevBlocks = [];
        var cutSheetNumber = 0;
        blocks = [];
        var placed = qNothing();


        while (size(sortedBlocks) > 0)
        {
            // Run binary tree bin-packing algorithm
            Packer(definition.width, definition.height, definition.spacing, sortedBlocks, cutSheetNumber, initialY);
            for (var i = 0; i < size(sortedBlocks); i += 1)
            {
                var block = sortedBlocks[i];

                // Move successfully placed parts to the binary tree bin packing location
                if (block[].fit != undefined)
                {
                    opTransform(context, id + ("transform_to_bin" ~ i ~ cutSheetNumber), {
                                "bodies" : block[].owner,
                                "transform" : block[].transform
                            });
                    placed = qUnion([placed, block[].owner]);
                }
                else
                {
                    blocks = append(blocks, block);
                }
            }
            // Sketch cut sheets if specified
            if (definition.showSheets)
            {
                var sketch1 = newSketch(context, id + ("sketch" ~ cutSheetNumber), {
                        "sketchPlane" : qCreatedBy(makeId("Top"), EntityType.FACE)
                    });
                var newX = cutSheetNumber * definition.width * 1.1;
                skRectangle(sketch1, "rectangle" ~ cutSheetNumber, {
                            "firstCorner" : vector(newX, initialY),
                            "secondCorner" : vector(definition.width + newX, initialY + definition.height)
                        });
                skSolve(sketch1);
            }
            // This condition checks that it is possible to space the parts, prevents infinite loop
            if (size(prevBlocks) != 0 && size(blocks) != 0 && prevBlocks == blocks)
            {
                throw regenError("Cut sheet is smaller than largest part plus twice the spacing", blocks[0][].owner);
            }
            else
            {
                // Here you are left with whatever parts don't fit on the first cut sheet.
                // This loop runs once for each cut sheet.
                prevBlocks = blocks;
                sortedBlocks = sortBlocks(blocks);
                cutSheetNumber += 1;
                blocks = [];
            }
        }

        // Mark the sucessfully-placed parts as having been placed by Auto Layout
        setAttribute(context, {
                    "entities" : placed,
                    "attribute" : "AutoLayout_PLACED" as AutoLayoutAttribute
                });

        setVariable(context, "AutoLayout_yinitial", initialY + definition.height * 1.1);
    });

// Sort parts based on heuristic metric
// Currently sorted by decreasing area
export function sortBlocks(blocks is array)
{
    var sortedBlocks = sort(blocks, function(block1, block2)
    {
        // if (max(block2[].w, block2[].h) != max(block1[].w, block1[].h))
        // {
        //     return max(block2[].w, block2[].h) - max(block1[].w, block1[].h);
        // }
        // else
        // {
        //     return min(block2[].w, block2[].h) - min(block1[].w, block1[].h);
        // }

        return (block2[].w * block2[].h - block1[].w * block1[].h);

        // var block1param = block1[].w * block1[].h * (1 + log(max(block1[].w, block1[].h) / min(block1[].w, block1[].h)));
        // var block2param = block2[].w * block2[].h * (1 + log(max(block2[].w, block2[].h) / min(block2[].w, block2[].h)));
        // return block2param - block1param;
    });
    return sortedBlocks;
}

// This is a helper function that computes transforms to rotate blocks in place so that
// they can be placed either vertically or horizontally on the cut sheet.
export function rotateBlock(block is box)
{
    var zaxis is Line = line(vector(0, 0, 0) * inch, vector(0, 0, 1));
    var rotateTransform = rotationAround(zaxis, 90 * degree);
    var transformToOrigin = transform(vector(block[].h, 0 * inch, 0 * inch));

    block[].transform = transformToOrigin * rotateTransform * block[].transform;
    block[].rotated = true;
}

// Modified binary tree bin packing from: https://github.com/jakesgordon/bin-packing/blob/master/js/packer.js

// Initializer for the bin packing algorithm
export function Packer(width is ValueWithUnits, height is ValueWithUnits, spacing is ValueWithUnits, blocks is array, cutSheetNumber, initialY is ValueWithUnits) returns array
{
    var root = new box({ 'x' : cutSheetNumber * width * 1.1 + spacing, 'y' : initialY + spacing, 'w' : width - 2 * spacing, 'h' : height - 2 * spacing, 'used' : false, 'rotated' : false, 'fitParam' : 0 * meter });
    return fit(root, blocks, spacing);
}

// Fit function calls findNode to determine recursively where the part fits on the sheet,
// then calls placeBlockAndSplit to create a bin above and a bin to the right
export function fit(root is box, blocks is array, spacing is ValueWithUnits) returns array
{
    var node;
    var block;
    for (var i = 0; i < size(blocks); i += 1)
    {
        block = blocks[i];
        node = findNode(root, block);

        if (node != undefined)
        {
            block[].fit = placeBlockAndSplit(node, block, spacing);
        }
        else
        {
            block[].fit = undefined;
        }
    }
    return blocks;
}

// Recursively finds a bin where the part will fit
export function findNode(root is box, block is box)
{
    var w = block[].w;
    var h = block[].h;

    if (root[].used)
    {
        var right = findNode(root[].right, block);
        var above = findNode(root[].above, block);
        if (right != undefined && above != undefined)
        {
            // Part can fit in a subnode somewhere to the right or somewhere above; choose the better one according to a heuristic
            // Currently chooses the placement minimizing maximum Y-coordinate
            if (above[].fitParam < right[].fitParam)
            {
                return above;
            }
            else
            {
                return right;
            }
        }
        else if (right != undefined)
        {
            return right;
        }
        else if (above != undefined)
        {
            return above;
        }
    }
    else // Find orientation within root that gives the tightest fit
    {
        var normalFit = undefined;
        var rotatedFit = undefined;

        if ((w < root[].w || tolerantEquals(w, root[].w)) && (h < root[].h || tolerantEquals(h, root[].h)))
        {
            // The part will fit in root without rotation
            normalFit = w + root[].x;
            // normalFit = root[].w - w;
        }

        if ((h < root[].w || tolerantEquals(h, root[].w)) && (w < root[].h || tolerantEquals(w, root[].h)))
        {
            // The part will fit in root with rotation
            rotatedFit = h + root[].x;
            // rotatedFit = root[].h - h;
        }

        if (normalFit != undefined && rotatedFit != undefined) //Part fits both ways, choose tighter fit
        {
            if (normalFit < rotatedFit || tolerantEquals(normalFit, rotatedFit))
            {
                root[].fitParam = normalFit;
                root[].rotated = false;
                return root;
            }
            else
            {
                root[].fitParam = rotatedFit;
                root[].rotated = true;
                return root;
            }
        }
        else if (normalFit != undefined)
        {
            root[].fitParam = normalFit;
            root[].rotated = false;
            return root;
        }
        else if (rotatedFit != undefined)
        {
            root[].fitParam = rotatedFit;
            root[].rotated = true;
            return root;
        }
        else
        {
            return undefined;
        }
    }
}

// Computes final transform on the part, splits the used node into one above it and one to the right of it
export function placeBlockAndSplit(node is box, block is box, spacing is ValueWithUnits) returns box
{
    if (node[].rotated)
    {
        rotateBlock(block);
    }
    var fitVector = vector(node[].x, node[].y, 0 * inch);
    var transformToBin = transform(fitVector);
    block[].transform = transformToBin * block[].transform;

    var w = block[].w;
    var h = block[].h;

    if (block[].rotated)
    {
        w = block[].h;
        h = block[].w;
    }

    node[].used = true;


    node[].above = new box({ 'x' : node[].x, 'y' : node[].y + h + spacing, 'w' : node[].w, 'h' : node[].h - (h + spacing), 'used' : false, 'rotated' : false, 'fitParam' : 0 * meter });
    node[].right = new box({ 'x' : node[].x + w + spacing, 'y' : node[].y, 'w' : node[].w - (w + spacing), 'h' : h + spacing, 'used' : false, 'rotated' : false, 'fitParam' : 0 * meter });


    return node;
}

export function getBoundingThickness(context is Context, body is Query)
{
    // Get largest planar face
    var face = getLargestFace(context, body);

    // If part has planar faces
    if (evaluateQuery(context, face) != [])
    {
        var largestFacePlane = evPlane(context, {
                "face" : face
            });
        const orientedCSys = planeToCSys(largestFacePlane);
        const bbox is Box3d = evBox3d(context, {
                    "topology" : body,
                    "cSys" : orientedCSys
                });
        return abs(bbox.maxCorner[2] - bbox.minCorner[2]);
    }
    else
    {
        return -1 * meter;
    }
}

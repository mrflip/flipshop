FeatureScript 2780;
import(path : "onshape/std/common.fs", version : "2780.0");

//---------------- ENUMS ----------------//
export enum Shape { Circle, Rectangle, Triangle, Arrow }

export enum LocalDebugColor { RED, GREEN, BLUE, CYAN, MAGENTA, YELLOW, BLACK, ORANGE }

//---------------- MAIN FEATURE ----------------//
annotation { "Feature Type Name" : "Debug Shapes Test", "Feature Type Description" : "" }
export const debugShapesTest = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Keep shapes" }
        definition.keepShapes is boolean;

        annotation { "Name" : "Widgets", "Item name" : "Widget" }
        definition.shapes is array;

        for (var widget in definition.shapes)
        {
            annotation { "Name" : "Shape" }
            widget.shape is Shape;

            annotation { "Name" : "Color" }
            widget.color is LocalDebugColor;

            annotation { "Name" : "Locations", "Filter" : BodyType.MATE_CONNECTOR }
            widget.locations is Query;

            if (widget.shape == Shape.Circle)
            {
                annotation { "Name" : "Radius" }
                isLength(widget.circleRadius, LENGTH_BOUNDS);
            }
            else if (widget.shape == Shape.Rectangle)
            {
                annotation { "Name" : "Width" }
                isLength(widget.width, LENGTH_BOUNDS);
                annotation { "Name" : "Height" }
                isLength(widget.height, LENGTH_BOUNDS);
                annotation { "Name" : "Corner Radius" }
                isLength(widget.rectangleCornerRadius, LENGTH_BOUNDS);
            }
            else if (widget.shape == Shape.Triangle)
            {
                annotation { "Name" : "Radius" }
                isLength(widget.triangleRadius, LENGTH_BOUNDS);
                annotation { "Name" : "Corner Radius" }
                isLength(widget.triangleCornerRadius, LENGTH_BOUNDS);
            }
            else if (widget.shape == Shape.Arrow)
            {
                annotation { "Name" : "Tail Length" }
                isLength(widget.arrowTailLength, LENGTH_BOUNDS);
                annotation { "Name" : "Head Length" }
                isLength(widget.arrowHeadLength, LENGTH_BOUNDS);
                annotation { "Name" : "Width (shaft)" }
                isLength(widget.arrowWidth, LENGTH_BOUNDS);
                annotation { "Name" : "Head Width" }
                isLength(widget.arrowHeadWidth, LENGTH_BOUNDS);
                annotation { "Name" : "Corner Radius" }
                isLength(widget.arrowCornerRadius, LENGTH_BOUNDS);
                annotation { "Name" : "From Tip" }
                widget.fromTip is boolean;
            }
        }
    }
    {
        for (var i = 0; i < size(definition.shapes); i += 1)
        {
            const sMap = definition.shapes[i];
            const locs = evaluateQuery(context, sMap.locations);

            for (var k = 0; k < size(locs); k += 1)
            {
                const csys = evMateConnector(context, { "mateConnector" : locs[k] });
                const color = DebugColor[toString(sMap.color)];
                const sid = id + unstableIdComponent(i) + k;

                if (sMap.shape == Shape.Circle)
                    DebugShapeCircle(context, sid, csys, sMap.circleRadius, color, definition.keepShapes);
                else if (sMap.shape == Shape.Rectangle)
                    DebugShapeRectangle(context, sid, csys, sMap.width, sMap.height, sMap.rectangleCornerRadius, color, definition.keepShapes);
                else if (sMap.shape == Shape.Triangle)
                    DebugShapeTriangle(context, sid, csys, sMap.triangleRadius, sMap.triangleCornerRadius, color, definition.keepShapes);
                else if (sMap.shape == Shape.Arrow)
                    DebugShapeArrow(context, sid, csys,
                        sMap.arrowTailLength, sMap.arrowWidth, sMap.arrowHeadLength, sMap.arrowHeadWidth,
                        sMap.arrowCornerRadius, color, definition.keepShapes, sMap.fromTip);
            }
        }
    });


//---------------- CIRCLE ----------------//
export function DebugShapeCircle(context is Context, id is Id,
    origin is CoordSystem, radius is ValueWithUnits,
    color is DebugColor, keepShape is boolean)
{
    const s = newSketchOnPlane(context, id + "skCircle", { "sketchPlane" : plane(origin) });
    skCircle(s, "circle", { "center" : vector(0, 0) * inch, "radius" : radius });
    skSolve(s);
    const edges = qCreatedBy(id + "skCircle", EntityType.EDGE);
    addDebugEntities(context, edges, color);
    if (!keepShape)
        opDeleteBodies(context, id + "delC", { "entities" : qCreatedBy(id + "skCircle") });
}


//---------------- RECTANGLE (inward fillets) ----------------//
export function DebugShapeRectangle(context is Context, id is Id,
    origin is CoordSystem, width is ValueWithUnits,
    height is ValueWithUnits, cornerRadius is ValueWithUnits,
    color is DebugColor, keepShape is boolean)
{
    const s = newSketchOnPlane(context, id + "skRect", { "sketchPlane" : plane(origin) });
    const w = width / 2;
    const h = height / 2;
    const r = min(max(cornerRadius, 0 * meter), min(w, h));

    if (r == 0 * meter)
    {
        skRectangle(s, "rect", { "firstCorner" : vector(-w, -h), "secondCorner" : vector(w, h) });
    }
    else
    {
        const p0 = vector(-w + r, -h);
        const p1 = vector(w - r, -h);
        const p2 = vector(w, -h + r);
        const p3 = vector(w, h - r);
        const p4 = vector(w - r, h);
        const p5 = vector(-w + r, h);
        const p6 = vector(-w, h - r);
        const p7 = vector(-w, -h + r);

        const q = r / sqrt(2);
        const mBR = vector(w - r + q, -h + r - q);
        const mTR = vector(w - r + q,  h - r + q);
        const mTL = vector(-w + r - q, h - r + q);
        const mBL = vector(-w + r - q, -h + r - q);

        skLineSegment(s, "b1", { "start" : p0, "end" : p1 });
        skArc(s, "a1", { "start" : p1, "mid" : mBR, "end" : p2 });

        skLineSegment(s, "b2", { "start" : p2, "end" : p3 });
        skArc(s, "a2", { "start" : p3, "mid" : mTR, "end" : p4 });

        skLineSegment(s, "b3", { "start" : p4, "end" : p5 });
        skArc(s, "a3", { "start" : p5, "mid" : mTL, "end" : p6 });

        skLineSegment(s, "b4", { "start" : p6, "end" : p7 });
        skArc(s, "a4", { "start" : p7, "mid" : mBL, "end" : p0 });
    }

    skSolve(s);
    const edges = qCreatedBy(id + "skRect", EntityType.EDGE);
    addDebugEntities(context, edges, color);
    if (!keepShape)
        opDeleteBodies(context, id + "delR", { "entities" : qCreatedBy(id + "skRect") });
}


//---------------- TRIANGLE (stable inward fillets) ----------------//
export function DebugShapeTriangle(context is Context, id is Id,
    origin is CoordSystem, radius is ValueWithUnits,
    filletR is ValueWithUnits, color is DebugColor, keepShape is boolean)
{
    const s = newSketchOnPlane(context, id + "skTri", { "sketchPlane" : plane(origin) });

    const R = radius;
    const v0 = vector(R * cos(90 * degree),  R * sin(90 * degree));
    const v1 = vector(R * cos(210 * degree), R * sin(210 * degree));
    const v2 = vector(R * cos(330 * degree), R * sin(330 * degree));
    const V = [v0, v1, v2];
    const L = norm(v1 - v0);
    const theta = 60 * degree;
    const r = min(max(filletR, 0 * meter), L * 0.45);

    if (r == 0 * meter)
    {
        skLineSegment(s, "e01", { "start" : v0, "end" : v1 });
        skLineSegment(s, "e12", { "start" : v1, "end" : v2 });
        skLineSegment(s, "e20", { "start" : v2, "end" : v0 });
    }
    else
    {
        const d = r / tan(theta / 2);
        var trims = [];
        for (var i = 0; i < 3; i += 1)
        {
            const A = V[i];
            const B = V[(i + 1) % 3];
            const C = V[(i + 2) % 3];
            const tAB = A + normalize(B - A) * d;
            const tAC = A + normalize(C - A) * d;
            trims = append(trims, { "A": A, "B": B, "C": C, "tAB": tAB, "tAC": tAC });
        }

        skLineSegment(s, "AB", { "start" : trims[0].tAB, "end" : trims[1].tAC });
        skLineSegment(s, "BC", { "start" : trims[1].tAB, "end" : trims[2].tAC });
        skLineSegment(s, "CA", { "start" : trims[2].tAB, "end" : trims[0].tAC });

        const sinHalf = sin(theta / 2);
        for (var i = 0; i < 3; i += 1)
        {
            const A = trims[i].A;
            const start = trims[i].tAC;
            const end = trims[i].tAB;
            const e1 = normalize(trims[i].B - A);
            const e2 = normalize(trims[i].C - A);
            const bis = normalize(e1 + e2);
            const h = r / sinHalf;
            const center = A + bis * h;
            const u = normalize(start - center);
            const v = normalize(end - center);
            const mid = center + normalize(u + v) * r;
            skArc(s, "f" ~ i, { "start" : start, "mid" : mid, "end" : end });
        }
    }

    skSolve(s);
    const edges = qCreatedBy(id + "skTri", EntityType.EDGE);
    addDebugEntities(context, edges, color);
    if (!keepShape)
        opDeleteBodies(context, id + "delT", { "entities" : qCreatedBy(id + "skTri") });
}


//---------------- ARROW (with tailLength + headLength + fromTip) ----------------//
export function DebugShapeArrow(context is Context, id is Id,
    origin is CoordSystem, tailLength is ValueWithUnits, shaftWidth is ValueWithUnits,
    headLength is ValueWithUnits, headWidth is ValueWithUnits, cornerRadius is ValueWithUnits,
    color is DebugColor, keepShape is boolean, fromTip is boolean)
{
    if (fromTip)
        origin = coordSystem(toWorld(origin, vector(-(tailLength + headLength), 0 * inch, 0 * inch)), origin.xAxis, origin.zAxis);

    const s = newSketchOnPlane(context, id + "skArr", { "sketchPlane" : plane(origin) });

    const TL = tailLength;
    const HL = headLength;
    const SW = shaftWidth / 2;
    const HW = headWidth / 2;
    const r = min(max(cornerRadius, 0 * meter), SW);

    const baseX = 0 * meter;
    const shaftEndX = TL;
    const tipX = TL + HL;

    const backBL = vector(baseX, -SW);
    const backTL = vector(baseX, SW);
    const shaftBR = vector(shaftEndX, -SW);
    const shaftTR = vector(shaftEndX, SW);
    const headBR = vector(shaftEndX, -HW);
    const headTR = vector(shaftEndX, HW);
    const tip = vector(tipX, 0 * meter);

    const q = r / sqrt(2);
    const mBL = vector(baseX + r - q, -SW + r - q);
    const mTL = vector(baseX + r - q, SW - r + q);

    skLineSegment(s, "shaftBottom", { "start" : vector(baseX + r, -SW), "end" : shaftBR });
    skLineSegment(s, "shaftTop", { "start" : shaftTR, "end" : vector(baseX + r, SW) });
    skLineSegment(s, "headBottom", { "start" : shaftBR, "end" : headBR });
    skLineSegment(s, "headFlankB", { "start" : headBR, "end" : tip });
    skLineSegment(s, "headFlankT", { "start" : tip, "end" : headTR });
    skLineSegment(s, "headTop", { "start" : headTR, "end" : shaftTR });

    if (r > 0 * meter)
    {
        skArc(s, "aTL", { "start" : vector(baseX + r, SW), "mid" : mTL, "end" : vector(baseX, SW - r) });
        skLineSegment(s, "backSide", { "start" : vector(baseX, SW - r), "end" : vector(baseX, -SW + r) });
        skArc(s, "aBL", { "start" : vector(baseX, -SW + r), "mid" : mBL, "end" : vector(baseX + r, -SW) });
    }
    else
        skLineSegment(s, "backSide", { "start" : backTL, "end" : backBL });

    skSolve(s);
    const edges = qCreatedBy(id + "skArr", EntityType.EDGE);
    addDebugEntities(context, edges, color);
    if (!keepShape)
        opDeleteBodies(context, id + "delA", { "entities" : qCreatedBy(id + "skArr") });
}

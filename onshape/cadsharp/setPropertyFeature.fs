//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Michael Pascoe and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Michael Pascoe.
//_______________________________________________________________________________________________________________________________________________

FeatureScript 2399;
import(path : "onshape/std/common.fs", version : "2399.0");
import(path : "onshape/std/frameAttributes.fs", version : "2105.0");

icon::import(path : "2d40f24a0d8d273369d7a89f", version : "273f344e863d8356cf5efa6c");
import(path : "c7c08274a0d273b9a5f5b47d/3bc427217b140370d222e3c6/f5c0f1a094e248af44b52910", version : "e0f8b228a335c29a22ea4719");
export import(path : "cbeb3dcf671e00785597bd76/409d65a3744fe434f32bdffc/a75ab01def146a42f55baa7f", version : "381046010d5aea697e433948");

export import(path : "c7c08274a0d273b9a5f5b47d/f7e4452612f025982491f803/0abb9be049d15f1839c40841", version : "565659f4f2db5ea5d7aeee26");

const NOT_REVISION_MANAGED_ID = "57f3fb8efa3416c06701d61d";
const UNIT_OF_MEASURE_ID = "57f3fb8efa3416c06701d623";

export enum ActionEnum
{
    annotation { "Name" : "Set" }
    SET,
    annotation { "Name" : "Get" }
    GET,
}

export enum OperationEnum
{
    annotation { "Name" : "Property" }
    PROPERTY,
    annotation { "Name" : "Attribute" }
    ATTRIBUTE,
    annotation { "Name" : "Frame cutlist attribute" }
    FRAME_CUTLIST_ATTRIBUTE,
    annotation { "Name" : "Identity" }
    IDENTITY,
    annotation { "Name" : "Transient id" }
    TRANSIENT_ID,
}

export enum FrameCutlistEnum
{
    annotation { "Name" : "Item" }
    Item,
    annotation { "Name" : "Qty" }
    Qty,
    annotation { "Name" : "Standard" }
    Standard,
    annotation { "Name" : "Description" }
    Description,
    annotation { "Name" : "Length" }
    Length,
    annotation { "Name" : "Angle 1" }
    Angle1,
    annotation { "Name" : "Angle 2" }
    Angle2
}

export enum MyUnitOfMeasure
{
    annotation { "Name" : "Centimeter" }
    Centimeter,
    annotation { "Name" : "Foot" }
    Foot,
    annotation { "Name" : "Inch" }
    Inch,
    annotation { "Name" : "Meter" }
    Meter,
    annotation { "Name" : "Millimeter" }
    Millimeter,
    annotation { "Name" : "Yard" }
    Yard,
    annotation { "Name" : "Gram" }
    Gram,
    annotation { "Name" : "Kilogram" }
    Kilogram,
    annotation { "Name" : "Ounce" }
    Ounce,
    annotation { "Name" : "Pound" }
    Pound,
    annotation { "Name" : "Liter" }
    Liter,
    annotation { "Name" : "Gallon" }
    Gallon,
    annotation { "Name" : "Each" }
    Each,
    annotation { "Name" : "Fluid ounce" }
    FluidOunce,
    annotation { "Name" : "Milliliter" }
    Milliliter,
    annotation { "Name" : "Centiliter" }
    Centiliter,
    annotation { "Name" : "Package" }
    Package,
}

export enum DensityUnits
{
    annotation { "Name" : "g / cm ^ 3" }
    G_CM,
    annotation { "Name" : "kg / cm ^ 3" }
    KG_CM,
    annotation { "Name" : "kg / m ^ 3" }
    KG_M,
    annotation { "Name" : "lb / in ^ 3" }
    LB_IN,
    annotation { "Name" : "lb / ft ^ 3" }
    LB_FT,
    annotation { "Name" : "oz / in ^ 3" }
    OZ_IN,
    annotation { "Name" : "oz / ft ^ 3" }
    OZ_FT
}

const DENSITY_CONSTANTS =
{
        DensityUnits.G_CM : gram / centimeter ^ 3,
        DensityUnits.KG_CM : kilogram / centimeter ^ 3,
        DensityUnits.KG_M : kilogram / meter ^ 3,
        DensityUnits.LB_FT : pound / foot ^ 3,
        DensityUnits.LB_IN : pound / inch ^ 3,
        DensityUnits.OZ_FT : ounce / foot ^ 3,
        DensityUnits.OZ_IN : ounce / inch ^ 3
    };

const UNIT_OF_MEASURE_CONSTANTS = {
        MyUnitOfMeasure.Centimeter : "Centimeter",
        MyUnitOfMeasure.Foot : "Foot",
        MyUnitOfMeasure.Inch : "Inch",
        MyUnitOfMeasure.Meter : "Meter",
        MyUnitOfMeasure.Millimeter : "Millimeter",
        MyUnitOfMeasure.Yard : "Yard",
        MyUnitOfMeasure.Gram : "Gram",
        MyUnitOfMeasure.Kilogram : "Kilogram",
        MyUnitOfMeasure.Ounce : "Ounce",
        MyUnitOfMeasure.Pound : "Pound",
        MyUnitOfMeasure.Liter : "Liter",
        MyUnitOfMeasure.Gallon : "Gallon",
        MyUnitOfMeasure.Each : "Each",
        MyUnitOfMeasure.FluidOunce : "Fluid ounce",
        MyUnitOfMeasure.Milliliter : "Milliliter",
        MyUnitOfMeasure.Centiliter : "Centiliter",
        MyUnitOfMeasure.Package : "Package",
    };


export enum MassUnitsEnum
{
    annotation { "Name" : "Gram" }
    gram,
    annotation { "Name" : "Kilogram" }
    kilogram,
    annotation { "Name" : "Ounce" }
    ounce,
    annotation { "Name" : "Pound" }
    pound,
}

// Imported from query finder
// export enum Property
// {
//     NAME,
//     MATERIAL,
//     APPEARANCE,
//     DESCRIPTION,
//     PART_NUMBER,
//     VENDOR,
//     PROJECT,
//     PRODUCT_LINE,
//     TITLE_1,
//     TITLE_2,
//     TITLE_3,
//     EXCLUDE_FROM_BOM,
//     CUSTOM,
//     MASS_OVERRIDE,
//     REVISION,
// }

export const suffixProperty = "Property";
export const suffixAttribute = "Attribute";
export const suffixFrameCutlistAttribute = "CutlistFrameAttribute";
export const suffixIdentity = "Identity";
export const suffixQuery = "Query";

annotation {
        "Feature Type Name" : "Property / Attribute",
        "Feature Name Template" : "#action #operation",
        "Icon" : icon::BLOB_DATA,
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Feature Type Description" : "<b> Summary </b> <br> Set the properties of a part. <br>",
        "Editing Logic Function" : "editLogic"
    }
export const property = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Action", "UIHint" : UIHint.HORIZONTAL_ENUM, "Default" : ActionEnum.SET }
        definition.action is ActionEnum;

        annotation { "Name" : "Operation", "UIHint" : [UIHint.REMEMBER_PREVIOUS_VALUE, UIHint.SHOW_LABEL] }
        definition.operation is OperationEnum;

        if (definition.operation == OperationEnum.PROPERTY)
        {
            annotation { "Name" : "Entities", "Filter" : (EntityType.BODY || EntityType.FACE) && AllowMeshGeometry.YES }
            definition.entities is Query;

            QFinderPredicate(definition, suffixProperty, "Entities");

            if (definition.action == ActionEnum.SET)
            {
                SetPropertyPredicate(definition);
            }
            else
            {
                annotation { "Name" : "Property to get", "UIHint" : UIHint.SHOW_LABEL }
                definition.propertyType is Property;

                if (definition.propertyType == Property.CUSTOM)
                {
                    annotation { "Name" : "Custom property id" }
                    definition.customPropertyId is string;
                }

                annotation { "Name" : "Variable to set", "Default" : "property" }
                definition.variableName is string;

                annotation { "Name" : "Property data", "UIHint" : [UIHint.READ_ONLY] }
                isAnything(definition.propertyData);

                annotation { "Name" : "Refresh" }
                isButton(definition.refresh);
            }
        }
        else if (definition.operation == OperationEnum.ATTRIBUTE)
        {
            annotation {
                        "Name" : "Entity",
                        "Filter" : (EntityType.BODY || EntityType.FACE || EntityType.VERTEX || EntityType.EDGE || BodyType.MATE_CONNECTOR),
                        "UIHint" : UIHint.PREVENT_CREATING_NEW_MATE_CONNECTORS
                    }
            definition.aEntity is Query;

            QFinderPredicate(definition, suffixAttribute, "Entities");

            annotation { "Name" : "Attribute name" }
            definition.aName is string;

            if (definition.action == ActionEnum.SET)
            {
                annotation { "Name" : "Attribute value" }
                definition.aValue is string;

                annotation { "Name" : "Color entity" }
                definition.aColorEntity is boolean;

                annotation { "Group Name" : "Color attribute", "Driving Parameter" : "aColorEntity", "Collapsed By Default" : false }
                {
                    if (definition.aColorEntity)
                    {
                        annotation { "Name" : "Hexadecimal", "Default" : "E2300B", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                        definition.aHexadecimal is string;
                    }
                }
            }
            else
            {
                annotation { "Name" : "Retreived value", "UIHint" : UIHint.READ_ONLY }
                definition.aRetreivedValue is string;

                annotation { "Name" : "Variable to set" }
                definition.aVariableName is string;
            }
        }
        else if (definition.operation == OperationEnum.FRAME_CUTLIST_ATTRIBUTE)
        {
            annotation {
                        "Name" : "Entity",
                        "Filter" : (EntityType.BODY),
                        "UIHint" : UIHint.PREVENT_CREATING_NEW_MATE_CONNECTORS
                    }
            definition.fEntity is Query;

            QFinderPredicate(definition, suffixFrameCutlistAttribute, "Entities");

            annotation { "Name" : "Cutlist attribute" }
            definition.cutlistAttribute is FrameCutlistEnum;

            if (definition.action == ActionEnum.SET)
            {
                annotation { "Name" : "Attribute value" }
                definition.fValue is string;

                annotation { "Name" : "Color entity" }
                definition.fColorEntity is boolean;

                annotation { "Group Name" : "Color attribute", "Driving Parameter" : "fColorEntity", "Collapsed By Default" : false }
                {
                    if (definition.fColorEntity)
                    {
                        annotation { "Name" : "Hexadecimal", "Default" : "E2300B", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
                        definition.fHexadecimal is string;
                    }
                }
            }
            else
            {
                annotation { "Name" : "Retreived value", "UIHint" : UIHint.READ_ONLY }
                definition.fRetreivedValue is string;

                annotation { "Name" : "Variable to set" }
                definition.fVariableName is string;
            }
        }
        else if (definition.operation == OperationEnum.IDENTITY)
        {
            if (definition.action == ActionEnum.SET)
            {
                annotation {
                            "Name" : "Entity",
                            "Filter" : (EntityType.BODY || EntityType.FACE || EntityType.VERTEX || EntityType.EDGE || BodyType.MATE_CONNECTOR),
                            "UIHint" : UIHint.PREVENT_CREATING_NEW_MATE_CONNECTORS,
                            "MaxNumberOfPicks" : 1 }
                definition.iEntity is Query;

                QFinderPredicate(definition, suffixIdentity, "Entities");

                annotation { "Name" : "Identity value" }
                definition.iValue is string;
            }
            else
            {
                annotation { "Name" : "Icon", "UIHint" : [UIHint.READ_ONLY, UIHint.DISPLAY_SHORT], "Default" : "⚠️" }
                definition.iIcon is string;

                annotation { "Name" : "Note", "UIHint" : [UIHint.READ_ONLY, UIHint.DISPLAY_SHORT], "Default" : "Use features with Query Finder for this operation. ⚠️" }
                definition.iNote is string;

                annotation { "Name" : ".", "UIHint" : UIHint.READ_ONLY }
                definition.iSpace is string;
            }
        }
        else if (definition.operation == OperationEnum.TRANSIENT_ID)
        {
            if (definition.action == ActionEnum.SET)
            {
                annotation { "Name" : "Icon", "UIHint" : [UIHint.READ_ONLY, UIHint.DISPLAY_SHORT], "Default" : "⚠️" }
                definition.qIcon is string;

                annotation { "Name" : "Note", "UIHint" : [UIHint.READ_ONLY, UIHint.DISPLAY_SHORT], "Default" : "Not available. ⚠️" }
                definition.qNote is string;

                annotation { "Name" : ".", "UIHint" : UIHint.READ_ONLY }
                definition.qSpace is string;

            }
            else
            {
                annotation {
                            "Name" : "Entity",
                            "Filter" : (EntityType.BODY || EntityType.FACE || EntityType.VERTEX || EntityType.EDGE || BodyType.MATE_CONNECTOR),
                            "UIHint" : UIHint.PREVENT_CREATING_NEW_MATE_CONNECTORS,
                            "MaxNumberOfPicks" : 1 }
                definition.qEntity is Query;

                QFinderPredicate(definition, suffixQuery, "Entities");

                annotation { "Name" : "Variable to set" }
                definition.qVariableName is string;

                annotation { "Name" : "Retreived value", "UIHint" : UIHint.READ_ONLY }
                definition.qRetreivedValue is string;
            }
        }

        cadsharpUrlPredicate(definition);
    }
    {
        definition.entities = qUnion([definition.entities, QFinderFunction(context, definition, suffixProperty)]);
        definition.aEntity = qUnion([definition.aEntity, QFinderFunction(context, definition, suffixAttribute)]);
        definition.iEntity = qUnion([definition.iEntity, QFinderFunction(context, definition, suffixIdentity)]);
        definition.qEntity = qUnion([definition.qEntity, QFinderFunction(context, definition, suffixQuery)]);

        const isGet = definition.action == ActionEnum.GET;
        const isProperty = definition.operation == OperationEnum.PROPERTY;

        if (definition.operation == OperationEnum.PROPERTY)
        {
            if (!isGet)
            {
                try
                {
                    setPropertiesFunction(context, id, definition);
                }
                catch
                {
                    if (!isQueryEmpty(context, qEntityFilter(definition.entities, EntityType.FACE)))
                    {
                        reportFeatureWarning(context, id, "Faces only allow certain properties to be set.");
                    }
                }
            }
            else
            {
                reportFeatureInfo(context, id, "WARNING:  Get PROPERTY is not live and only updates when you click the refresh button!");
                setVariable(context, definition.variableName, definition.propertyData);
            }
        }
        else if (definition.operation == OperationEnum.ATTRIBUTE)
        {
            if (!isGet)
            {
                if (!isQueryEmpty(context, qBodyType(definition.aEntity, BodyType.MATE_CONNECTOR)))
                {
                    // When selecting mate connectors, and a vertex is also available, it uses a mate connector vertex instead of the body
                    definition.aEntity = definition.aEntity->qOwnerBody();
                }

                setAttribute(context, {
                            "entities" : definition.aEntity,
                            "name" : definition.aName,
                            "attribute" : definition.aValue
                        });

                if (definition.aColorEntity)
                {
                    const hex = hexToRGB(definition.aHexadecimal);

                    setProperty(context, {
                                "entities" : definition.aEntity,
                                "propertyType" : PropertyType.APPEARANCE,
                                "value" : color(hex.red, hex.green, hex.blue, 1)
                            });
                }
            }
            else
            {
                var attribute = undefined;

                try
                {
                    attribute = getAttribute(context, {
                                "entity" : definition.aEntity,
                                "name" : definition.aName
                            });
                }

                if (attribute == undefined)
                {
                    attribute = "No attribute found.";
                }
                else
                {
                    try
                    {
                        setVariable(context, definition.aVariableName, attribute);
                        reportFeatureInfo(context, id, attribute->toString());
                    }
                    catch
                    {
                        reportFeatureInfo(context, id, "Variable not set.");
                    }
                }
            }
        }
        else if (definition.operation == OperationEnum.FRAME_CUTLIST_ATTRIBUTE)
        {
            if (!isGet)
            {
                if (!isQueryEmpty(context, qBodyType(definition.fEntity, BodyType.MATE_CONNECTOR)))
                {
                    // When selecting mate connectors, and a vertex is also available, it uses a mate connector vertex instead of the body
                    definition.fEntity = definition.fEntity->qOwnerBody();
                }

                const frameComposite = qCompositePartsContaining(definition.fEntity);

                var cutlist = getAttribute(context, {
                        "entity" : frameComposite,
                        "name" : FRAME_ATTRIBUTE_CUTLIST_NAME
                    });

                cutlist = setValueInCutlist(context, cutlist, definition.fEntity, definition.cutlistAttribute, definition.fValue);

                // debug(context, cutlist, DebugColor.RED);

                // setAttribute(context, {
                //             "entities" : frameComposite,
                //             "name" : FRAME_ATTRIBUTE_CUTLIST_NAME,
                //             "attribute" : cutlist
                //         });

                setCutlistAttribute(context, frameComposite, cutlist as CutlistAttribute);



                if (definition.aColorEntity)
                {
                    const hex = hexToRGB(definition.aHexadecimal);

                    setProperty(context, {
                                "entities" : definition.aEntity,
                                "propertyType" : PropertyType.APPEARANCE,
                                "value" : color(hex.red, hex.green, hex.blue, 1)
                            });
                }
            }
            else
            {
                var attribute = undefined;

                try
                {
                    const frameComposite = qCompositePartsContaining(definition.fEntity);

                    const cutlist = getAttribute(context, {
                                "entity" : frameComposite,
                                "name" : FRAME_ATTRIBUTE_CUTLIST_NAME
                            });

                    attribute = getValueFromCutlist(context, cutlist, definition.fEntity, definition.cutlistAttribute);
                }

                if (attribute == undefined)
                {
                    attribute = "No attribute found.";
                }
                else
                {
                    try
                    {
                        setVariable(context, definition.fVariableName, attribute);
                        reportFeatureInfo(context, id, attribute->toString());
                    }
                    catch
                    {
                        reportFeatureInfo(context, id, "Variable not set.");
                    }
                }
            }
        }
        else if (definition.operation == OperationEnum.IDENTITY)
        {
            if (!isGet)
            {
                reportFeatureInfo(context, id, "Manually forces the identity of your part so that it does not change.");

                if (!isQueryEmpty(context, definition.iEntity))
                {
                    if (!isQueryEmpty(context, qBodyType(definition.iEntity, BodyType.MATE_CONNECTOR)))
                    {
                        definition.iEntity = definition.iEntity->qOwnerBody();
                    }

                    opNameEntity(context, id, { "entity" : definition.iEntity, "entityName" : definition.iValue });

                }
            }
            else
            {
                reportFeatureInfo(context, id, "⚠️ Use features with Query Finder for this operation. ⚠️");
                // reportFeatureInfo(context, id, "In FeatureScript, use qNamed(\"yourIdentity\") to retreive the part with the identity you set. ");
            }
        }
        else if (definition.operation == OperationEnum.TRANSIENT_ID)
        {
            if (!isGet)
            {
                reportFeatureInfo(context, id, "⚠️ This action is not available. Transient id's are set by Onshape. ⚠️");
            }
            else
            {
                if (!isQueryEmpty(context, qBodyType(definition.qEntity, BodyType.MATE_CONNECTOR)))
                {
                    definition.qEntity = definition.qEntity->qOwnerBody();
                }

                const transientId = transientQueriesToStrings(definition.qEntity);
                setVariable(context, definition.qVariableName, transientId);

                // const transientQuery = transientId as Query;
                // debug(context, transientQuery, DebugColor.RED);
            }
        }

        setFeatureComputedParameter(context, id, {
                    "name" : "action",
                    "value" : isGet ? "[Get]" : "[Set]"
                });

        const operationName = switch (definition.operation) {
                    OperationEnum.PROPERTY : "- Property",
                    OperationEnum.ATTRIBUTE : "- Attribute",
                    OperationEnum.IDENTITY : "- Identity",
                };

        setFeatureComputedParameter(context, id, {
                    "name" : "operation",
                    "value" : operationName
                });
    });

function setPropertiesFunction(context is Context, id is Id, definition is map)
{
    reportFeatureInfo(context, id, "TIP:  Right click on an input to convert to expression.  This will let you use variables.");

    if (definition.nameBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.NAME,
                    "value" : definition.name
                });
    }

    if (definition.materialBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.MATERIAL,
                    "value" : material(definition.materialName, definition.materialDensity * DENSITY_CONSTANTS[definition.densityUnits])
                });
    }

    if (definition.appearanceBool)
    {
        const hex = hexToRGB(definition.hexadecimal);

        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.APPEARANCE,
                    "value" : color(hex.red, hex.green, hex.blue, 1)
                });
    }

    if (definition.descriptionBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.DESCRIPTION,
                    "value" : definition.description
                });
    }

    if (definition.partNumberBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.PART_NUMBER,
                    "value" : definition.partNumber
                });
    }

    if (definition.vendorBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.VENDOR,
                    "value" : definition.vendor
                });
    }

    if (definition.projectBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.PROJECT,
                    "value" : definition.project
                });
    }

    if (definition.productLineBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.PRODUCT_LINE,
                    "value" : definition.productLine
                });
    }

    if (definition.title1Bool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.TITLE_1,
                    "value" : definition.title1
                });
    }

    if (definition.title2Bool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.TITLE_2,
                    "value" : definition.title2
                });
    }

    if (definition.title3Bool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.TITLE_3,
                    "value" : definition.title3
                });
    }

    if (definition.massOverrideBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.MASS_OVERRIDE,
                    "value" : definition.massValue ~ " " ~ definition.massUnits
                });
    }

    if (definition.excludeFromBOMBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.EXCLUDE_FROM_BOM,
                    "value" : definition.excludeFromBOM
                });
    }

    if (definition.notRevisionManagedBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.CUSTOM,
                    "customPropertyId" : NOT_REVISION_MANAGED_ID,
                    "value" : definition.notRevisionManaged->toString()
                });
    }

    // Read only..
    // if (definition.revisionBool)
    // {
    //     setProperty(context, {
    //                 "entities" : definition.entities,
    //                 "propertyType" : PropertyType.REVISION,
    //                 "value" : definition.revision
    //             });
    // }

    if (definition.unitOfMeasureBool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.CUSTOM,
                    "customPropertyId" : UNIT_OF_MEASURE_ID,
                    "value" : UNIT_OF_MEASURE_CONSTANTS[definition.unitOfMeasure]
                });
    }

    if (definition.custom1Bool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.CUSTOM,
                    "customPropertyId" : definition.custom1Id,
                    "value" : definition.custom1Value
                });
    }

    if (definition.custom2Bool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.CUSTOM,
                    "customPropertyId" : definition.custom2Id,
                    "value" : definition.custom2Value
                });
    }

    if (definition.custom3Bool)
    {
        setProperty(context, {
                    "entities" : definition.entities,
                    "propertyType" : PropertyType.CUSTOM,
                    "customPropertyId" : definition.custom3Id,
                    "value" : definition.custom3Value
                });
    }
}


export predicate SetPropertyPredicate(definition)
{
    annotation { "Name" : "Name" }
    definition.nameBool is boolean;

    annotation { "Group Name" : "Name Group", "Driving Parameter" : "nameBool", "Collapsed By Default" : false }
    {
        if (definition.nameBool)
        {
            annotation { "Name" : "Name", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.name is string;
        }
    }

    annotation { "Name" : "Material" }
    definition.materialBool is boolean;

    annotation { "Group Name" : "Material Group", "Driving Parameter" : "materialBool", "Collapsed By Default" : false }
    {
        if (definition.materialBool)
        {
            annotation { "Name" : "Material Name", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.materialName is string;

            annotation { "Name" : "Material Density" }
            isReal(definition.materialDensity, { (unitless) : [0, 1, 1e9] } as RealBoundSpec);

            annotation { "Name" : "Material Units", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.densityUnits is DensityUnits;
        }
    }

    annotation { "Name" : "Appearance" }
    definition.appearanceBool is boolean;

    annotation { "Group Name" : "Appearance Group", "Driving Parameter" : "appearanceBool", "Collapsed By Default" : false }
    {
        if (definition.appearanceBool)
        {
            annotation { "Name" : "Hexadecimal", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.hexadecimal is string;
        }
    }

    annotation { "Name" : "Description" }
    definition.descriptionBool is boolean;

    annotation { "Group Name" : "Description Group", "Driving Parameter" : "descriptionBool", "Collapsed By Default" : false }
    {
        if (definition.descriptionBool)
        {
            annotation { "Name" : "Description", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.description is string;
        }
    }

    annotation { "Name" : "Part Number" }
    definition.partNumberBool is boolean;

    annotation { "Group Name" : "Part Number Group", "Driving Parameter" : "partNumberBool", "Collapsed By Default" : false }
    {
        if (definition.partNumberBool)
        {
            annotation { "Name" : "Part Number", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.partNumber is string;
        }
    }

    annotation { "Name" : "Vendor" }
    definition.vendorBool is boolean;

    annotation { "Group Name" : "Vendor Group", "Driving Parameter" : "vendorBool", "Collapsed By Default" : false }
    {
        if (definition.vendorBool)
        {
            annotation { "Name" : "Vendor", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.vendor is string;
        }
    }

    annotation { "Name" : "Project" }
    definition.projectBool is boolean;

    annotation { "Group Name" : "Project Group", "Driving Parameter" : "projectBool", "Collapsed By Default" : false }
    {
        if (definition.projectBool)
        {
            annotation { "Name" : "Project", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.project is string;
        }
    }

    annotation { "Name" : "Product Line" }
    definition.productLineBool is boolean;

    annotation { "Group Name" : "Product Line Group", "Driving Parameter" : "productLineBool", "Collapsed By Default" : false }
    {
        if (definition.productLineBool)
        {
            annotation { "Name" : "Product Line", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.productLine is string;
        }
    }

    annotation { "Name" : "Title 1" }
    definition.title1Bool is boolean;

    annotation { "Group Name" : "Title 1 Group", "Driving Parameter" : "title1Bool", "Collapsed By Default" : false }
    {
        if (definition.title1Bool)
        {
            annotation { "Name" : "Title 1", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.title1 is string;
        }
    }

    annotation { "Name" : "Title 2" }
    definition.title2Bool is boolean;

    annotation { "Group Name" : "Title 2 Group", "Driving Parameter" : "title2Bool", "Collapsed By Default" : false }
    {
        if (definition.title2Bool)
        {
            annotation { "Name" : "Title 2", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.title2 is string;
        }
    }

    annotation { "Name" : "Title 3" }
    definition.title3Bool is boolean;

    annotation { "Group Name" : "Title 3 Group", "Driving Parameter" : "title3Bool", "Collapsed By Default" : false }
    {
        if (definition.title3Bool)
        {
            annotation { "Name" : "Title 3", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.title3 is string;
        }
    }

    annotation { "Name" : "Mass Override" }
    definition.massOverrideBool is boolean;

    annotation { "Group Name" : "Mass Override Group", "Driving Parameter" : "massOverrideBool", "Collapsed By Default" : false }
    {
        if (definition.massOverrideBool)
        {
            annotation { "Name" : "Mass Units", "UIHint" : [UIHint.SHOW_LABEL, UIHint.REMEMBER_PREVIOUS_VALUE] }
            definition.massUnits is MassUnitsEnum;

            annotation { "Name" : "Mass Value" }
            definition.massValue is string;
        }
    }

    annotation { "Name" : "Exclude From BOM" }
    definition.excludeFromBOMBool is boolean;

    annotation { "Group Name" : "Exclude From BOM Group", "Driving Parameter" : "excludeFromBOMBool", "Collapsed By Default" : false }
    {
        if (definition.excludeFromBOMBool)
        {
            annotation { "Name" : "Exclude From BOM", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.excludeFromBOM is boolean;
        }
    }

    annotation { "Name" : "Not Revision Managed" }
    definition.notRevisionManagedBool is boolean;

    annotation { "Group Name" : "Not Revision Managed Group", "Driving Parameter" : "notRevisionManagedBool", "Collapsed By Default" : false }
    {
        if (definition.notRevisionManagedBool)
        {
            annotation { "Name" : "Not Revision Managed", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.notRevisionManaged is boolean;
        }
    }

    // Read only..
    // annotation { "Name" : "Revision" }
    // definition.revisionBool is boolean;

    // annotation { "Group Name" : "Revision Group", "Driving Parameter" : "revisionBool", "Collapsed By Default" : false }
    // {
    //     if (definition.revisionBool)
    //     {
    //         annotation { "Name" : "Revision", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
    //         definition.revision is string;
    //     }
    // }

    annotation { "Name" : "Unit of Measure" }
    definition.unitOfMeasureBool is boolean;

    annotation { "Group Name" : "Unit of Measure Group", "Driving Parameter" : "unitOfMeasureBool", "Collapsed By Default" : false }
    {
        if (definition.unitOfMeasureBool)
        {
            annotation { "Name" : "Unit of Measure", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.unitOfMeasure is MyUnitOfMeasure;
        }
    }

    annotation { "Name" : "Custom 1" }
    definition.custom1Bool is boolean;

    annotation { "Group Name" : "Custom 1 Group", "Driving Parameter" : "custom1Bool", "Collapsed By Default" : false }
    {
        if (definition.custom1Bool)
        {
            annotation { "Name" : "Property Id", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.custom1Id is string;

            annotation { "Name" : "Value", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.custom1Value is string;
        }
    }

    annotation { "Name" : "Custom 2" }
    definition.custom2Bool is boolean;

    annotation { "Group Name" : "Custom 2 Group", "Driving Parameter" : "custom2Bool", "Collapsed By Default" : false }
    {
        if (definition.custom2Bool)
        {
            annotation { "Name" : "Property Id", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.custom2Id is string;

            annotation { "Name" : "Value", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.custom2Value is string;
        }
    }

    annotation { "Name" : "Custom 3" }
    definition.custom3Bool is boolean;

    annotation { "Group Name" : "Custom 3 Group", "Driving Parameter" : "custom3Bool", "Collapsed By Default" : false }
    {
        if (definition.custom3Bool)
        {
            annotation { "Name" : "Property Id", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.custom3Id is string;

            annotation { "Name" : "Value", "UIHint" : UIHint.REMEMBER_PREVIOUS_VALUE }
            definition.custom3Value is string;
        }
    }
}

function getValueFromCutlist(context, cutlist, query is Query, columnEnum)
{
    const columnId = enumToCutlistColumnName[columnEnum];
    const rows = cutlist.table.rows;
    const rowCount = size(rows);

    // Get entities from the input query
    const inputEntities = evaluateQuery(context, query);
    const inputEntityCount = size(inputEntities);

    for (var i = 0; i < rowCount; i += 1)
    {
        const row = rows[i];
        const rowEntities = row.entities;

        if (!isQueryEmpty(context, qIntersection([query, rowEntities])))
        {
            const columnMap = row.columnIdToCell;
            const keys = keys(columnMap);

            return columnMap[enumToCutlistColumnName[columnEnum]];
        }
    }
}

function setValueInCutlist(context, cutlist, query is Query, columnEnum, newValue)
{
    const rowCount = size(cutlist.table.rows);
    const columnKey = columnEnum->toString();

    for (var i = 0; i < rowCount; i += 1)
    {
        if (!isQueryEmpty(context, qIntersection([query, cutlist.table.rows[i].entities])))
        {
            cutlist.table.rows[i].columnIdToCell[enumToCutlistColumnName[columnEnum]] = newValue;
            return cutlist;
        }
    }

    throw "Matching row not found to set value.";
}


const enumToCutlistColumnName =
{
        FrameCutlistEnum.Item : "Item",
        FrameCutlistEnum.Qty : "Qty",
        FrameCutlistEnum.Standard : "Standard",
        FrameCutlistEnum.Description : "Description",
        FrameCutlistEnum.Length : "Length",
        FrameCutlistEnum.Angle1 : "Angle 1",
        FrameCutlistEnum.Angle2 : "Angle 2"
    };


// By lana from Onshape
function toIdentity(id is Id) returns string
{
    var out is string = id[0];
    for (var i = 1; i < size(id); i += 1)
    {
        out ~= "." ~ id[i];
    }
    return out;
}

function setQFinderDefaults(context is Context, definition is map, oldDefinition is map)
{
    // property
    definition = QFinderSetDefaultsAndVisibility(context, definition, oldDefinition, {
                "showExclude" : true,
                "showFilterEntities" : true,
                "showIncludeConstructionEntities" : false,
                "showIncludeSketchEntities" : false,
                "showWires" : false,
                "showVertices" : false,
                "showEdges" : false,
                "showSurfaces" : false,
                "showFaces" : true,
                "showSolids" : true,
                "showComposites" : true,
                "showMateConnectors" : true
            }, {
                "queryFinder" : false,
                "searchType" : SearchType.ATTRIBUTE,
                "filterEntities" : false,
                "includeConstructionEntities" : false,
                "includeSketchEntities" : false,
                "wires" : false,
                "vertices" : false,
                "edges" : false,
                "surfaces" : false,
                "faces" : false,
                "solids" : true,
                "composites" : false,
                "mateConnectors" : false
            }, suffixProperty);

    // attribute, identity, query
    const toSet = [suffixAttribute, suffixIdentity, suffixQuery];
    for (var i = 0; i < size(toSet); i += 1)
    {
        definition = QFinderSetDefaultsAndVisibility(context, definition, oldDefinition, {
                    "showExclude" : true,
                    "showFilterEntities" : true,
                    "showIncludeConstructionEntities" : false,
                    "showIncludeSketchEntities" : false,
                    "showWires" : true,
                    "showVertices" : true,
                    "showEdges" : true,
                    "showSurfaces" : true,
                    "showFaces" : true,
                    "showSolids" : true,
                    "showComposites" : true,
                    "showMateConnectors" : true
                }, {
                    "queryFinder" : false,
                    "searchType" : SearchType.ATTRIBUTE,
                    "filterEntities" : false,
                    "includeConstructionEntities" : false,
                    "includeSketchEntities" : false,
                    "wires" : false,
                    "vertices" : false,
                    "edges" : false,
                    "surfaces" : false,
                    "faces" : false,
                    "solids" : true,
                    "composites" : false,
                    "mateConnectors" : false
                }, toSet[i]);
    }

    // frame cutlist attribute
    definition = QFinderSetDefaultsAndVisibility(context, definition, oldDefinition, {
                "showExclude" : true,
                "showFilterEntities" : true,
                "showIncludeConstructionEntities" : false,
                "showIncludeSketchEntities" : false,
                "showWires" : false,
                "showVertices" : false,
                "showEdges" : false,
                "showSurfaces" : false,
                "showFaces" : false,
                "showSolids" : true,
                "showComposites" : false,
                "showMateConnectors" : false
            }, {
                "queryFinder" : false,
                "searchType" : SearchType.ATTRIBUTE,
                "filterEntities" : false,
                "includeConstructionEntities" : false,
                "includeSketchEntities" : false,
                "wires" : false,
                "vertices" : false,
                "edges" : false,
                "surfaces" : false,
                "faces" : false,
                "solids" : true,
                "composites" : false,
                "mateConnectors" : false
            }, suffixFrameCutlistAttribute);

    return definition;
}

export function editLogic(context is Context, id is Id, oldDefinition is map, definition is map, isCreating is boolean, specifiedParameters is map, clickedButton is string) returns map
{
    definition = cadsharpUrlFunctionForPreExistingEditLogic(oldDefinition, definition);
    definition = setQFinderDefaults(context, definition, oldDefinition);

    if (definition.action == ActionEnum.GET)
    {
        if (definition.operation == OperationEnum.PROPERTY)
        {
            var propertyDef = {};
            propertyDef.entity = qNthElement(definition.entities, 0);
            propertyDef.propertyType = PropertyType[definition.propertyType->toString()];

            if (definition.propertyType == Property.CUSTOM)
            {
                propertyDef.customPropertyId = definition.customPropertyId;
            }

            definition.propertyData = getProperty(context, propertyDef);

            try
            {
                var test = getProperty(context, {
                        "entity" : propertyDef.entity,
                        "customPropertyId" : "651c08085947d02b666d5f1e"
                    });

                println(test);
            }
        }
        else if (definition.operation == OperationEnum.ATTRIBUTE)
        {
            try silent
            {
                definition.aRetreivedValue = getAttribute(context, {
                            "entity" : definition.aEntity,
                            "name" : definition.aName
                        });
            }
        }
        else if (definition.operation == OperationEnum.FRAME_CUTLIST_ATTRIBUTE)
        {
            // try silent
            // {
            //     definition.fRetreivedValue = getAttribute(context, {
            //                 "entity" : definition.fEntity,
            //                 "name" : definition.fName
            //             });
            // }
        }
        else if (definition.operation == OperationEnum.IDENTITY)
        {
            try silent
            {
                // definition.aRetreivedValue = getAttribute(context, {
                //             "entity" : definition.iEntity,
                //             "name" : definition.iName
                //         });


            }
        }
        else if (definition.operation == OperationEnum.TRANSIENT_ID)
        {
            try silent
            {
                definition.qRetreivedValue = transientQueriesToStrings(definition.qEntity)->toString();
            }
        }
    }
    else
    {
        if (definition.operation == OperationEnum.IDENTITY)
        {
            // Auto fill the value with an id
            if (specifiedParameters.iValue != true)
                definition.iValue = toIdentity(id);
        }
    }

    return definition;
}



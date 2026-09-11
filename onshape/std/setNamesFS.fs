FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "14a20c5c0c7e0354a621f347/185949b8bdfeb193e13253d7/6fcd20533bd2df7c4094a0bf", version : "566d908d563f598d3989a764");
import(path : "14a20c5c0c7e0354a621f347/185949b8bdfeb193e13253d7/4ebdc64943b566160ea5cc28", version : "332a1753347ec00e4732ac5f");

// /**
//  * Sets the Parts-list name of whatever `query` resolves to.
//  */
// export function setReadableName(context is Context, query is Query, name is string) returns string {
//   setProperty(context, {
//     "entities" :     query,
//     "propertyType" : PropertyType.NAME,
//     "value" :       name,
//   });
//   return name
// }

export enum SetNameMode {
    annotation { "Name": "Simple" }
    SIMPLE,
    annotation { "Name": "Bulk" }
    BULK,
    annotation { "Name": "Series" }
    SERIES,
}

annotation { "Feature Type Name":     "Set name",
             "Feature Name Template": "#displayTitle" }
export const setNameFS = defineFeature(function(context is Context, id is Id, definition is map)
precondition {

    annotation {
        "Name":     "Mode",
        "Default":  SetNameMode.SIMPLE,
        "UIHint":   [UIHint.REMEMBER_PREVIOUS_VALUE, UIHint.SHOW_LABEL],
    }
    definition.mode is SetNameMode;

    if (definition.mode == SetNameMode.SIMPLE) {
      annotation {
        "Name":    "Part",
        "Filter":  EntityType.BODY,
        "UIHint":  [UIHint.INITIAL_FOCUS],
        "MaxNumberOfPicks": 1,
      }
      definition.thingQ is Query;
    } else if (definition.mode == SetNameMode.BULK) {
      annotation {
        "Name":      "Part Names",
        "Item name": "Part Name",
        "UIHint":     [UIHint.FOCUS_INNER_QUERY],
      }
      definition.partNames is array;

      for (var row in definition.partNames) {
        annotation {
          "Name":    "Part",
          "Filter":  EntityType.BODY,
          "UIHint":   [UIHint.FOCUS_ON_VISIBLE],
          "MaxNumberOfPicks": 1,
        }
        row.rowThingQ is Query;

        annotation {
          "Name":    "Name",
          "Default":  "Thing",
        }
        row.name is string;
      }
    }

    if (definition.mode != SetNameMode.BULK) {
        annotation {
          "Name":     "Name",
          "Default":  "Thing",
          "UIHint":   [UIHint.INITIAL_FOCUS_ON_EDIT],
        }
        definition.newName is string;
    }
    if (definition.mode == SetNameMode.SERIES) {
        annotation {
          "Name":     "Pad",
          "Default":  1,
          "UIHint":   [UIHint.REMEMBER_PREVIOUS_VALUE],
        }
        isInteger(definition.pad, { (unitless) : [0, 1, 6] } as IntegerBoundSpec);
    }

    annotation { "Name" : "Feature Display Title", "UIHint" : [UIHint.ALWAYS_HIDDEN] } // UIHint.READ_ONLY
    definition.displayTitle is string;
}
{
    const mode = definition.mode;
    setNamesFSTitle(context, id, definition);

    if (strBlank(definition.newName)) {
        throw regenError("Enter a name", ["newName"]);
    }
    var thingsQ = [];
    if (definition.mode == SetNameMode.SIMPLE) {
      setReadableName(context, definition.thingQ, definition.newName, 99);
    } else if (definition.mode == SetNameMode.SERIES) {
      var ii = 0;
      for (var qq in evaluateQuery(context, thingsQ)) {
        const suffix = padLeft(strSlice(ii, 0, definition.pad), definition.pad, "0");
        setReadableName(context, qq, definition.newName ~ ' ' ~ suffix);
        ii += 1;
      }
    } else if (definition.mode == SetNameMode.BULK) {
      for (var partName in definition.partNames) {
        if (isQueryBlank(context, partName.rowThingQ)) { continue; }
        try {
          setReadableName(context, partName.rowThingQ, partName.name, 99);
        } catch (err) { debug(context, "Error in setting name" ~ err ~ ' for ' ~ partName); }
      }
    }

});

function setNamesFSTitle(context is Context, id is Id, definition is map) returns string {
    var displayTitle = "";
    if (definition.mode == SetNameMode.BULK) {
        displayTitle = 'Set Names ' ~ join(mapArray(ifNil(definition.partNames, []), (partName) => (
            isQueryBlank(context, partName.rowThingQ) ? '-' : partName.name
        )), ',');
    } else if (definition.mode == SetNameMode.SERIES) {
        displayTitle = 'Set Name ' ~ definition.newName ~ '…';
    } else {
        displayTitle = 'Set Name ' ~ definition.newName;
    }
    debug(context, displayTitle);
    setFeatureComputedParameter(context, id, { name: "displayTitle", value: displayTitle });
    return displayTitle;
}

export function mapArrayField(arr is array, fieldname is string) {
  return mapArray(arr, (val) => val[fieldname]);
}

export const identityFunc = (val) => val;

export function isQueryBlank(context is Context, query is Query) { return isNil(query) || isQueryEmpty(context, query); }

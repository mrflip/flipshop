FeatureScript 3070;
import(path : "onshape/std/common.fs", version : "3070.0");
import(path : "14a20c5c0c7e0354a621f347/185949b8bdfeb193e13253d7/6fcd20533bd2df7c4094a0bf", version : "566d908d563f598d3989a764");
import(path : "14a20c5c0c7e0354a621f347/185949b8bdfeb193e13253d7/4ebdc64943b566160ea5cc28", version : "332a1753347ec00e4732ac5f");

export const MaxNameLen = 99;

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
precondition
{

  annotation {
    "Name":    "Mode",
    "Default": SetNameMode.SIMPLE,
    "UIHint":  [UIHint.REMEMBER_PREVIOUS_VALUE, UIHint.SHOW_LABEL],
  }
  definition.mode is SetNameMode;

  if (definition.mode == SetNameMode.SIMPLE) {
    annotation {
      "Name":             "Part",
      "Filter":           EntityType.BODY,
      "UIHint":           [UIHint.INITIAL_FOCUS],
      "MaxNumberOfPicks": 1,
    }
    definition.thingQ is Query;
  } else if (definition.mode == SetNameMode.BULK) {
    annotation {
      "Name":      "Part Names",
      "Item name": "Part Name",
      "UIHint":    [UIHint.FOCUS_INNER_QUERY],
    }
    definition.partNames is array;

    for (var row in definition.partNames) {
      annotation {
        "Name":             "Part",
        "Filter":           EntityType.BODY,
        "UIHint":           [UIHint.FOCUS_ON_VISIBLE],
        "MaxNumberOfPicks": 1,
      }
      row.rowThingQ is Query;

      annotation {
        "Name":    "Name",
        "Default": "Thing",
      }
      row.name is string;
    }
  } else if (definition.mode == SetNameMode.SERIES) {
    annotation {
      "Name":   "Parts",
      "Filter": EntityType.BODY,
      "UIHint": [UIHint.INITIAL_FOCUS],
    }
    definition.thingsQ is Query;
  }

  if (definition.mode != SetNameMode.BULK) {
    annotation {
      "Name":    "Name",
      "Default": "Thing",
      "UIHint":  [UIHint.INITIAL_FOCUS_ON_EDIT],
    }
    definition.newName is string;
  }
  if (definition.mode == SetNameMode.SERIES) {
    annotation {
      "Name":    "Pad",
      "Default": 1,
      "UIHint":  [UIHint.REMEMBER_PREVIOUS_VALUE],
    }
    isInteger(definition.pad, { (unitless) : [0, 1, 6] } as IntegerBoundSpec);
  }

  annotation { "Name": "Feature Display Title", "UIHint": [UIHint.ALWAYS_HIDDEN] }
  definition.displayTitle is string;
}
{
  const mode = definition.mode;
  setNamesFSTitle(context, id, definition);

  if (mode == SetNameMode.BULK) {
    for (var partName in definition.partNames) {
      if (isQueryBlank(context, partName.rowThingQ)) { continue; }
      try {
        setReadableName(context, partName.rowThingQ, partName.name, MaxNameLen);
      } catch (err) { debug(context, 'Error setting name ' ~ err ~ ' for ' ~ partName); }
    }
  } else {
    if (strBlank(definition.newName)) {
      throw regenError('Enter a name', ['newName']);
    }
    if (mode == SetNameMode.SIMPLE) {
      setReadableName(context, definition.thingQ, definition.newName, MaxNameLen);
    } else {
      var ii = 1;
      for (var qq in evaluateQuery(context, definition.thingsQ)) {
        setReadableName(context, qq, definition.newName ~ seriesSuffix(ii, definition.pad), MaxNameLen);
        ii += 1;
      }
    }
  }
});

function setNamesFSTitle(context is Context, id is Id, definition is map) returns string {
  var displayTitle = '';
  if (definition.mode == SetNameMode.BULK) {
    displayTitle = 'Set Names ' ~ join(mapArray(ifNil(definition.partNames, []), (partName) => (
      isQueryBlank(context, partName.rowThingQ) ? '-' : partName.name
    )), ',');
  } else if (definition.mode == SetNameMode.SERIES) {
    displayTitle = 'Set Name ' ~ definition.newName ~ '…';
  } else {
    displayTitle = 'Set Name ' ~ definition.newName;
  }
  setFeatureComputedParameter(context, id, { "name": "displayTitle", "value": displayTitle });
  return displayTitle;
}

function seriesSuffix(index is number, pad is number) returns string {
  if (pad == 0) { return ''; }
  return ' ' ~ padLeft(toString(index), pad, '0');
}

export function mapArrayField(arr is array, fieldname is string) {
  return mapArray(arr, (val) => val[fieldname]);
}

export const identityFunc = (val) => val;

export function isQueryBlank(context is Context, qy is Query) { return isNil(qy) || isQueryEmpty(context, qy); }


//_______________________________________________________________________________________________________________________________________________
//
// This FeatureScript is owned by Konstantin and is distributed by CADSharp LLC.
// You may not redistribute it for commercial purposes without the permission of said owner and CADSharp LLC. Copyright (c) 2023 Konstantin.
//_______________________________________________________________________________________________________________________________________________


FeatureScript 718;
import(path : "onshape/std/geometry.fs", version : "718.0");
icon::import(path : "f40162bb65da8ea899309601", version : "4669d63d07be6806f44ec3ae");

// CADSharp
export import(path : "cbeb3dcf671e00785597bd76/144bf6a7fdc989e9e28ce5ea/a75ab01def146a42f55baa7f", version : "dc78e9b85c9f16ea9e131d3f");

/**
   Алгоритм нахождения максимальной выпуклой оболочки множества 3Д точек

   Находим верхнюю точку множества P1;

   Находим точку P2, как дающую минимальный угол отклонения опорной плоскости от плоскости заданной точкой P1 и векторами X и Y;

   Находим точку P3, как дающую минимальный угол отклонения опорной плоскости от плоскости заданной точкой P1 и векторами P1P2 и Y;

   Добавляем ребро P1P2 в список_пройденных_ребер;

   Добавляем ребра P2P3 и P3P1 с нормалями исходных граней в список_ребер_на_проверку;

   Инициализируем список_граней = [ грань P1P2P3 ];

   Повтор пока список_ребер_на_проверку не пуст:
   {
   Буфер =[];
   Для ребра PiPj из список_ребер_на_проверку:
   {
   Найти точку Pk функцией поиска точки, минимизирующую отклонение нормали от нормали исходной грани;
   Если PiPk не в списке_пройденных_ребер или в Буфере то добавить PiPk в Буфер;
   Если PjPi не в списке_пройденных_ребер или в Буфере то добавить PjPi в Буфер;
   Добавить ребро PiPj в список_пройденных_ребер;
   Добавить грань PiPjPk в список_граней;
   }
   список_ребер_на_проверку = Буфер;
   }
   Построение оболочки по списку_граней
 **/

annotation {
        "Feature Type Name" : "Convex polyhedron",
        "Icon" : icon::BLOB_DATA,
        "Feature Type Description" : "<b> Summary </b> <br> Creates a convex polyhedron from vertices. <br>",
        "Description Image" : cadsharpLogo::BLOB_DATA,
        "Editing Logic Function" : "cadsharpUrlEditLogic", }
export const convexPolyhedron = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
        annotation { "Name" : "Vertices", "Filter" : EntityType.VERTEX }
        definition.vertices is Query;

        cadsharpUrlPredicate(definition);
    }
    {
        //Функция сравнения точек
        const pointCompFunc = function(p1, p2)
            {
                return tolerantEquals(p1, p2);
            };

        // Нахождение координат вершин
        var pointArray = [];
        for (var vertex in evaluateQuery(context, definition.vertices))
        {
            var point = evVertexPoint(context, { "vertex" : vertex });
            //Добавляются только не дублирующиеся точки
            if (!isIn(point, pointArray, pointCompFunc))
            {
                pointArray = append(pointArray, point);
            }
        }
        //Нахождение верхней точки массива
        var estimationFunction = function(p)
            {
                return p[2];
            };
        const P1 = maxItem(pointArray, estimationFunction);
        //Определение вспомогательной точки P0
        const P0 = P1 + vector(0, 1, 0) * meter;
        //Формируем вспомогательное справочное ребро
        var refEdge = { "point1" : P1, "point2" : P0, "normal" : vector(0, 0, -1) };
        //Находим точку P2
        var P2 = getPoint(refEdge, pointArray);
        //Формируем ребро P1P2
        refEdge = { "point1" : P1, "point2" : P2, "normal" : cross(P2 - P1, P0 - P1) };
        //Находим точку P3
        var P3 = getPoint(refEdge, pointArray);
        //Формируем первчиный список ребер на проверку
        var checkList = createEdges(refEdge, P3);
        //Формируем первичный список пройденных ребер
        var checkedList = [refEdge];
        //Формируем список граней
        var faceList = [[P1, P2, P3]];

        //Начинаем главный цикл, пока список ребер на проверку не пуст
        while (checkList != [])
        {
            var edgeBuffer = [];

            for (var refEdge in checkList)
            {
                var P = getPoint(refEdge, pointArray);
                var newEdgesPair = createEdges(refEdge, P);
                var unitedCheckedList = concatenateArrays([checkedList, edgeBuffer]);
                for (var newEdge in newEdgesPair)
                {
                    if (!isEdgeIn(newEdge, unitedCheckedList))
                    {
                        edgeBuffer = append(edgeBuffer, newEdge);
                    }
                }
                checkedList = append(checkedList, refEdge);
                faceList = append(faceList, [refEdge.point1, refEdge.point2, P]);
            }
            checkList = edgeBuffer;
        }

        var i = 0;
        var queryList = [];
        for (var face in faceList)
        {
            i += 1;
            opTriangle(context, id + i + "triangle", face);
            queryList = append(queryList, qCreatedBy(id + i + "triangle"));
        }
        queryList = qUnion(queryList);

        enclose(context, id + "enclose", { "entities" : qEntityFilter(queryList, EntityType.FACE) });

        opDeleteBodies(context, id + "deleteBodies", { "entities" : queryList });
    });

//Проверка наличия элемента в массиве с помощью пользовательской функции сравнения
function isIn(value, container is array, compareFunction is function) returns boolean
{
    for (var element in container)
    {
        if (compareFunction(element, value))
            return true;
    }
    return false;
}

function maxItem(container is array, estimationFunction is function)
{
    var max = container[0];
    for (var item in container)
    {
        if (estimationFunction(item) > estimationFunction(max))
        {
            max = item;
        }
    }
    return max;
}

/*
   Функция вычисления угла между опорной и секущей плоскостями
   Положительное направление поворота - относиельно оси Z локальной системы координат
   ось Z - refVector, ось X - refNormal - внутреннее направление нормали, диапазон угла отклонения -90...+90 град
   сортировка на максимальное приближение к 90 градусам
 */
function pointEstimationFunction(refPoint is Vector, refVector is Vector, refNormal is Vector, point is Vector) returns number
{
    refVector = normalize(refVector);
    refNormal = normalize(refNormal);
    var refPlane = plane(refPoint, refVector, refNormal);
    var currentVector = worldToPlane(refPlane, point);
    currentVector = normalize(currentVector);
    return currentVector[1];
}

/*
   Функция поиска точки из массива, дающей минимальное отклонение нормали секущей плоскости относительно заданного ребра
   Требуется исключение из списка для поиска точки, лежащие на линии текущего ребра
 */
function getPoint(refEdge is map, points is array) returns Vector
{
    const refPoint = refEdge.point1;
    var refVector = refEdge.point2 - refEdge.point1;
    refVector = normalize(refVector);
    const refNormal = refEdge.normal;

    //Исключение из списка точек текущего ребра или лежащих на текущем ребре
    const filterFunction = function(p)
        {
            //Фильтруем точки, которые лежат за пределами трубки вокруг ребра
            return norm(cross(p - refPoint, refVector)) > TOLERANCE.zeroLength * millimeter;
        };
    points = filter(points, filterFunction);

    const estimationFunction = function(p)
        {
            return pointEstimationFunction(refPoint, refVector, refNormal, p);
        };
    //Выбираем точку, максимизирующую функцию оценки
    return maxItem(points, estimationFunction);
}

//Функция сравнения двух величин типа "ребро"
function isSameEdges(edge1 is map, edge2 is map) returns boolean
{
    return isIn(edge1.point1, [edge2.point1, edge2.point2]) &&
        isIn(edge1.point2, [edge2.point1, edge2.point2]);
}

//Функция проверки наличия ребра в списке пройденных или буферризированных
function isEdgeIn(refEdge is map, checkList is array) returns boolean
{
    for (var edge in checkList)
    {
        if (isSameEdges(edge, refEdge))
            return true;
    }
    return false;
}

//Функция, возвращающая список двух новых ребер [P1P3, P3P2] по ребру P1P2 и найденной точке P3
function createEdges(refEdge is map, newPoint is Vector) returns array
{
    const P1 = refEdge.point1;
    const P2 = refEdge.point2;
    const P3 = newPoint;
    //Нахождение внутренней нормали грани
    const refNormal = cross(P3 - P1, P2 - P1);

    const refEdge1 = { "point1" : P1, "point2" : P3, "normal" : refNormal };
    const refEdge2 = { "point1" : P3, "point2" : P2, "normal" : refNormal };

    return [refEdge1, refEdge2];
}

//Функция, создающая треугольную грань
function opTriangle(context is Context, id is Id, pointList is array)
{
    //Определяем плоскость эскиза
    const workPlaneNormal = cross(pointList[1] - pointList[0], pointList[2] - pointList[0]);
    const workhPlane = plane(pointList[0], workPlaneNormal);
    var point2dList = [];
    for (var point3d in pointList)
    {
        const point2d = worldToPlane(workhPlane, point3d);
        point2dList = append(point2dList, point2d);
    }
    point2dList = append(point2dList, point2dList[0]);
    var sk = newSketchOnPlane(context, id + "triangle", { "sketchPlane" : workhPlane });

    skPolyline(sk, "triangle", { "points" : point2dList });
    skSolve(sk);
}

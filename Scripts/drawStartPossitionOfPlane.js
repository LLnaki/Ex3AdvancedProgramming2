/**
 *This function draw a point on some context ctx with coordinates xOfCircle, yOfCircle.
 * as a blue-red circle.
 */
drawStartPossitionOfPlane = function (ctx, xOfCicle, yOfCircle) {
    var radiusOfBlueCircle = 10;
    var radiusOfRedCircle = 6;

    ctx.beginPath();
    ctx.arc(xOfCicle, yOfCircle, radiusOfBlueCircle, 0, 2 * Math.PI);
    ctx.fillStyle = "blue";
    ctx.fill();
    ctx.closePath();
    ctx.beginPath();
    ctx.arc(xOfCicle, yOfCircle, radiusOfRedCircle, 0, 2 * Math.PI);
    ctx.fillStyle = "red";
    ctx.fill();
    ctx.closePath();
}
/**
 * This function normilizes a longtitude(x coordinate) value so that
 * it can be properly displayed on a canvas.
 * @param {any} x - longtitude of a plain
 * @param {any} htmlCanvas - canvas on  which a point is intended to be shown
 */
function normilizeX(x, htmlCanvas) {
    return (x + 180) * htmlCanvas.width / 360;
}
/**
 * This function normilizes a latitude(y coordinate) value so that
 * it can be properly displayed on a canvas.
 * @param {any} x - latitude of a plain
 * @param {any} htmlCanvas - canvas on  which a point is intended to be shown
 */
function normilizeY(y,htmlCanvas) {
    return (y + 90) * htmlCanvas.height / 180;
}
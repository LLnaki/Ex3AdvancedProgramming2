        /**
         * This function configurates htmlCanvas canvas so that it always has an exact sizes of a view page. That is,
         * so that it covers all the screen. Also, it alowes to window to be resized by user and still  htmCanvas
         * will have a suitable to a page size.
         * Pay attention!!In order to achive such resizibility, there are still things that 
         * should be configured seperatedly in a style, and they are not handled  by this function! Namely, the next code
         * should be added as style:
         *      html, body {
         *          width: 100%;
         *          height: 100%;
         *          margin: 0px;
         *          border: 0;
         *          overflow: hidden; 
         *          display: block;
         *      }
         *And for htmlCanas style should be:
         *      style='position:absolute; left:0px; top:0px;'.
         */
        adjustBackground = function (htmlCanvas, mainFunc) {
        // Obtain a reference to the canvas element using its id.
        // Start listening to resize events and draw canvas.
        initialize();

        function initialize() {
            // Register an event listener to call the resizeCanvas() function
            // each time the window is resized.
            window.addEventListener('resize', resizeCanvas, false);
            window.addEventListener('resize', mainFunc, false);
            // Draw canvas border for the first time.
            resizeCanvas();
            mainFunc();
            }

            // Runs each time the DOM window resize event fires.
            // Resets the canvas dimensions to match window,
            // then draws the new borders accordingly.
            function resizeCanvas() {
                htmlCanvas.width = window.innerWidth;
                htmlCanvas.height = window.innerHeight;
            }
};

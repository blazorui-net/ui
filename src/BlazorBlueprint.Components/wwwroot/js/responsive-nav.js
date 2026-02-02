/**
 * ResponsiveNav JavaScript module
 * Handles mobile detection for responsive navigation
 */

const MOBILE_BREAKPOINT = 768;

let dotNetRef = null;
let resizeObserver = null;

/**
 * Initialize responsive nav with mobile detection
 * @param {DotNetObject} componentRef - Reference to the ResponsiveNavProvider component
 */
export function initialize(componentRef) {
    dotNetRef = componentRef;
    setupMobileDetection();
}

/**
 * Set up mobile detection using ResizeObserver
 */
function setupMobileDetection() {
    if (!dotNetRef) return;

    const checkMobile = () => {
        const isMobile = window.innerWidth < MOBILE_BREAKPOINT;
        dotNetRef.invokeMethodAsync('OnMobileChange', isMobile);
    };

    // Initial check
    checkMobile();

    // Listen for resize events
    resizeObserver = new ResizeObserver(() => {
        checkMobile();
    });

    resizeObserver.observe(document.body);
}

/**
 * Cleanup event listeners and observers
 */
export function cleanup() {
    if (resizeObserver) {
        resizeObserver.disconnect();
        resizeObserver = null;
    }

    dotNetRef = null;
}

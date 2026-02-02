// Range Slider drag handler
// Handles mouse drag events for dual-thumb slider

const rangeSliderStates = new Map();

/**
 * Initializes drag handling for a range slider
 * @param {HTMLElement} trackElement - The slider track element
 * @param {DotNetObject} dotNetRef - Reference to the Blazor component
 * @param {string} sliderId - Unique identifier for the slider
 */
export function initializeRangeSlider(trackElement, dotNetRef, sliderId) {
    if (!trackElement || !dotNetRef) {
        console.error('initializeRangeSlider: missing required parameters');
        return;
    }

    const state = {
        trackElement,
        dotNetRef,
        isDragging: false,
        activeThumb: null // 'start' or 'end'
    };

    const calculatePercentage = (clientX) => {
        const rect = trackElement.getBoundingClientRect();
        return Math.max(0, Math.min(1, (clientX - rect.left) / rect.width));
    };

    const handleMouseMove = (e) => {
        if (!state.isDragging || !state.activeThumb) return;
        e.preventDefault();

        const percentage = calculatePercentage(e.clientX);
        state.dotNetRef.invokeMethodAsync('UpdateValueFromPercentage', percentage, state.activeThumb).catch(err => {
            console.error('Error updating range slider value:', err);
        });
    };

    const handleMouseUp = () => {
        if (state.isDragging) {
            state.isDragging = false;
            state.activeThumb = null;
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
            document.body.style.userSelect = '';
            document.body.style.cursor = '';
        }
    };

    const handleTrackMouseDown = (e) => {
        // Only handle clicks on the track, not on thumbs (thumbs handle their own events)
        if (e.target.hasAttribute('data-thumb')) return;

        e.preventDefault();
        const percentage = calculatePercentage(e.clientX);

        // Get current percentages from the component
        state.dotNetRef.invokeMethodAsync('HandleTrackClick', percentage).catch(err => {
            console.error('Error handling track click:', err);
        });
    };

    // Attach mousedown to the track element
    trackElement.addEventListener('mousedown', handleTrackMouseDown);

    // Store for cleanup and external access
    rangeSliderStates.set(sliderId, {
        state,
        handleTrackMouseDown,
        handleMouseMove,
        handleMouseUp,
        trackElement
    });
}

/**
 * Starts dragging a specific thumb
 * @param {string} sliderId - Unique identifier for the slider
 * @param {string} thumb - Which thumb ('start' or 'end')
 */
export function startDrag(sliderId, thumb) {
    const stored = rangeSliderStates.get(sliderId);
    if (!stored) return;

    const { state, handleMouseMove, handleMouseUp } = stored;

    state.isDragging = true;
    state.activeThumb = thumb;

    document.body.style.userSelect = 'none';
    document.body.style.cursor = 'grabbing';

    document.addEventListener('mousemove', handleMouseMove);
    document.addEventListener('mouseup', handleMouseUp);
}

/**
 * Removes range slider handling
 * @param {string} sliderId - Unique identifier for the slider
 */
export function disposeRangeSlider(sliderId) {
    const stored = rangeSliderStates.get(sliderId);
    if (stored) {
        stored.trackElement.removeEventListener('mousedown', stored.handleTrackMouseDown);
        document.removeEventListener('mousemove', stored.handleMouseMove);
        document.removeEventListener('mouseup', stored.handleMouseUp);
        rangeSliderStates.delete(sliderId);
    }
}

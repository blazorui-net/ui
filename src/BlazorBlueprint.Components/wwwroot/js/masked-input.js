/**
 * Masked Input JavaScript interop module.
 * Handles cursor positioning that Blazor can't manage during active typing.
 */

/**
 * Sets the value of an input element and positions the cursor.
 * @param {HTMLInputElement} element - The input element.
 * @param {string} value - The value to set.
 * @param {number} cursorPosition - Where to position the cursor.
 */
export function setInputValue(element, value, cursorPosition) {
    if (!element) return;

    element.value = value;

    // Set cursor position
    if (cursorPosition >= 0 && cursorPosition <= value.length) {
        element.setSelectionRange(cursorPosition, cursorPosition);
    }
}

/**
 * Sets the cursor position in an input element.
 * @param {HTMLInputElement} element - The input element.
 * @param {number} cursorPosition - Where to position the cursor.
 */
export function setCursorPosition(element, cursorPosition) {
    if (!element) return;

    // Set cursor position immediately
    if (cursorPosition >= 0 && cursorPosition <= element.value.length) {
        element.setSelectionRange(cursorPosition, cursorPosition);
    }
}

/**
 * Gets the current cursor position in an input element.
 * @param {HTMLInputElement} element - The input element.
 * @returns {number} The cursor position.
 */
export function getCursorPosition(element) {
    if (!element) return 0;
    return element.selectionStart || 0;
}

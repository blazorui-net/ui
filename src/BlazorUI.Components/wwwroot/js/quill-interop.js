// Quill.js interop for RichTextEditor component
// Handles editor initialization, events, and content management

let editorStates = new Map();

/**
 * Initializes a Quill editor instance
 * @param {HTMLElement} element - The editor container element
 * @param {DotNetObject} dotNetRef - Reference to the Blazor component
 * @param {string} editorId - Unique identifier for the editor
 * @param {Object} options - Editor configuration options
 */
export function initializeEditor(element, dotNetRef, editorId, options) {
    if (!element || !dotNetRef) {
        console.error('initializeEditor: missing required parameters');
        return;
    }

    if (typeof Quill === 'undefined') {
        console.error('Quill is not loaded. Please include Quill.js in your page.');
        return;
    }

    const quillOptions = {
        theme: null,  // Headless mode - we handle the toolbar ourselves
        placeholder: options.placeholder || '',
        readOnly: options.readOnly || false,
        modules: {
            toolbar: false  // We build our own toolbar in Blazor
        },
        // Explicitly register all formats we support
        formats: [
            'bold', 'italic', 'underline', 'strike',
            'header',
            'list',
            'blockquote', 'code-block',
            'link',
            'indent'
        ]
    };

    const quill = new Quill(element, quillOptions);

    // Debounced text-change handler
    let textChangeTimeout;
    const textChangeHandler = (delta, oldDelta, source) => {
        clearTimeout(textChangeTimeout);
        textChangeTimeout = setTimeout(() => {
            // Check if callbacks are suppressed (during programmatic updates)
            const state = editorStates.get(editorId);
            if (state && state.suppressCallbacks) {
                return;
            }

            // Use getSemanticHTML for normalized output (Quill 2.0+)
            const html = typeof quill.getSemanticHTML === 'function'
                ? quill.getSemanticHTML()
                : quill.root.innerHTML;

            dotNetRef.invokeMethodAsync('OnTextChangeCallback', {
                delta: JSON.stringify(delta),
                oldDelta: JSON.stringify(oldDelta),
                source: source,
                html: html,
                text: quill.getText(),
                length: quill.getLength()
            }).catch(err => console.error('Error in text-change:', err));
        }, 150);
    };
    quill.on('text-change', textChangeHandler);

    // Selection-change for focus/blur detection and format tracking
    const selectionChangeHandler = (range, oldRange, source) => {
        const format = range ? quill.getFormat(range) : {};
        dotNetRef.invokeMethodAsync('OnSelectionChangeCallback', {
            range: range,
            oldRange: oldRange,
            source: source,
            format: format
        }).catch(err => console.error('Error in selection-change:', err));
    };
    quill.on('selection-change', selectionChangeHandler);

    editorStates.set(editorId, {
        quill,
        dotNetRef,
        textChangeTimeout,
        textChangeHandler,
        selectionChangeHandler,
        suppressCallbacks: false
    });
}

/**
 * Disposes of an editor instance
 * @param {string} editorId - Unique identifier for the editor
 */
export function disposeEditor(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        // Clear pending debounce timeout
        clearTimeout(stored.textChangeTimeout);

        // Remove event handlers to prevent memory leaks
        stored.quill.off('text-change', stored.textChangeHandler);
        stored.quill.off('selection-change', stored.selectionChangeHandler);

        editorStates.delete(editorId);
    }
}

/**
 * Sets the HTML content of the editor
 * Uses Quill's clipboard module to properly convert HTML to Delta,
 * maintaining synchronization between DOM and internal state.
 * Suppresses callbacks to prevent update loops during programmatic updates.
 * @param {string} editorId - Unique identifier for the editor
 * @param {string} html - HTML content to set
 */
export function setHtml(editorId, html) {
    const stored = editorStates.get(editorId);
    if (stored) {
        const quill = stored.quill;

        // Suppress callbacks during programmatic update
        stored.suppressCallbacks = true;

        try {
            if (!html) {
                // Empty content - set empty Delta
                quill.setContents([{ insert: '\n' }], 'api');
            } else {
                // Convert HTML to Delta using Quill's clipboard module
                const delta = quill.clipboard.convert({ html: html });
                quill.setContents(delta, 'api');
            }
        } finally {
            // Re-enable callbacks after a short delay to allow any pending events to be suppressed
            setTimeout(() => {
                stored.suppressCallbacks = false;
            }, 200);
        }
    }
}

/**
 * Gets the HTML content of the editor using Quill's semantic HTML output
 * Uses getSemanticHTML() for normalized, consistent HTML across browsers
 * @param {string} editorId - Unique identifier for the editor
 * @returns {string} HTML content
 */
export function getHtml(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        // Use getSemanticHTML for normalized output (Quill 2.0+)
        // Falls back to innerHTML if not available
        if (typeof stored.quill.getSemanticHTML === 'function') {
            return stored.quill.getSemanticHTML();
        }
        return stored.quill.root.innerHTML;
    }
    return '';
}

/**
 * Sets the editor contents using a Delta object
 * Suppresses callbacks to prevent update loops during programmatic updates.
 * @param {string} editorId - Unique identifier for the editor
 * @param {string} delta - JSON string representation of the Delta
 */
export function setContents(editorId, delta) {
    const stored = editorStates.get(editorId);
    if (stored && delta) {
        // Suppress callbacks during programmatic update
        stored.suppressCallbacks = true;

        try {
            stored.quill.setContents(JSON.parse(delta), 'api');
        } finally {
            // Re-enable callbacks after a short delay to allow any pending events to be suppressed
            setTimeout(() => {
                stored.suppressCallbacks = false;
            }, 200);
        }
    }
}

/**
 * Gets the editor contents as a Delta object
 * @param {string} editorId - Unique identifier for the editor
 * @returns {string} JSON string representation of the Delta
 */
export function getContents(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        return JSON.stringify(stored.quill.getContents());
    }
    return '{}';
}

/**
 * Gets the plain text content of the editor
 * @param {string} editorId - Unique identifier for the editor
 * @returns {string} Plain text content
 */
export function getText(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        return stored.quill.getText();
    }
    return '';
}

/**
 * Gets the length of the editor content
 * @param {string} editorId - Unique identifier for the editor
 * @returns {number} Content length
 */
export function getLength(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        return stored.quill.getLength();
    }
    return 0;
}

/**
 * Gets the current selection range
 * @param {string} editorId - Unique identifier for the editor
 * @returns {Object|null} Selection range with index and length, or null
 */
export function getSelection(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        return stored.quill.getSelection();
    }
    return null;
}

/**
 * Sets the selection range
 * @param {string} editorId - Unique identifier for the editor
 * @param {number} index - Start index
 * @param {number} length - Selection length
 */
export function setSelection(editorId, index, length) {
    const stored = editorStates.get(editorId);
    if (stored) {
        stored.quill.setSelection(index, length);
    }
}

/**
 * Applies formatting to the current selection
 * @param {string} editorId - Unique identifier for the editor
 * @param {string} formatName - Name of the format
 * @param {*} value - Format value
 */
export function format(editorId, formatName, value) {
    const stored = editorStates.get(editorId);
    if (stored) {
        stored.quill.format(formatName, value);
    }
}

/**
 * Applies formatting and returns the updated format state
 * Used for all formats to ensure immediate state sync
 * @param {string} editorId - Unique identifier for the editor
 * @param {string} formatName - Name of the format
 * @param {*} value - Format value
 * @returns {Object} Updated format state
 */
export function formatAndGetState(editorId, formatName, value) {
    const stored = editorStates.get(editorId);
    if (stored) {
        const quill = stored.quill;
        const range = quill.getSelection();

        // Special handling for block format removal in Quill v2
        // Quill's format('code-block', false) and format('blockquote', false) don't work correctly
        // We need to preserve inline formats when removing block formats
        if ((formatName === 'code-block' || formatName === 'blockquote') && value === false && range) {
            const [line, offset] = quill.getLine(range.index);
            if (line) {
                const lineIndex = quill.getIndex(line);
                const lineLength = line.length();

                // Get the Delta for this line to preserve inline formats
                const lineDelta = quill.getContents(lineIndex, lineLength);

                // Collect inline formats from each operation in the line
                const inlineFormats = [];
                let currentIndex = lineIndex;

                for (const op of lineDelta.ops) {
                    if (op.insert && typeof op.insert === 'string' && op.attributes) {
                        // Filter to only inline formats (not block formats)
                        const inlineAttrs = {};
                        const inlineFormatNames = ['bold', 'italic', 'underline', 'strike', 'link', 'code'];
                        for (const key of inlineFormatNames) {
                            if (op.attributes[key] !== undefined) {
                                inlineAttrs[key] = op.attributes[key];
                            }
                        }
                        if (Object.keys(inlineAttrs).length > 0) {
                            inlineFormats.push({
                                index: currentIndex,
                                length: op.insert.length,
                                formats: inlineAttrs
                            });
                        }
                    }
                    if (op.insert) {
                        currentIndex += typeof op.insert === 'string' ? op.insert.length : 1;
                    }
                }

                // Remove all formatting from the line (this removes the block format)
                quill.removeFormat(lineIndex, lineLength, 'api');

                // Re-apply the inline formats we saved
                for (const fmt of inlineFormats) {
                    for (const [key, val] of Object.entries(fmt.formats)) {
                        quill.formatText(fmt.index, fmt.length, key, val, 'api');
                    }
                }
            }
        } else {
            quill.format(formatName, value);
        }

        // Get the updated format state immediately after applying
        const newRange = quill.getSelection();
        return newRange ? quill.getFormat(newRange) : quill.getFormat();
    }
    return {};
}

/**
 * Gets the formatting at the current selection
 * @param {string} editorId - Unique identifier for the editor
 * @returns {Object} Format object
 */
export function getFormat(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        return stored.quill.getFormat();
    }
    return {};
}

/**
 * Enables the editor
 * @param {string} editorId - Unique identifier for the editor
 */
export function enable(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        stored.quill.enable(true);
    }
}

/**
 * Disables the editor
 * @param {string} editorId - Unique identifier for the editor
 */
export function disable(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        stored.quill.enable(false);
    }
}

/**
 * Focuses the editor
 * @param {string} editorId - Unique identifier for the editor
 */
export function focus(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        stored.quill.focus();
    }
}

/**
 * Removes focus from the editor
 * @param {string} editorId - Unique identifier for the editor
 */
export function blur(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        stored.quill.blur();
    }
}


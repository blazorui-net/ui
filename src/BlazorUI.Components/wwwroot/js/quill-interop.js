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
    quill.on('text-change', (delta, oldDelta, source) => {
        clearTimeout(textChangeTimeout);
        textChangeTimeout = setTimeout(() => {
            dotNetRef.invokeMethodAsync('OnTextChangeCallback', {
                delta: JSON.stringify(delta),
                oldDelta: JSON.stringify(oldDelta),
                source: source,
                html: quill.root.innerHTML,
                text: quill.getText(),
                length: quill.getLength()
            }).catch(err => console.error('Error in text-change:', err));
        }, 150);
    });

    // Selection-change for focus/blur detection and format tracking
    quill.on('selection-change', (range, oldRange, source) => {
        const format = range ? quill.getFormat(range) : {};
        dotNetRef.invokeMethodAsync('OnSelectionChangeCallback', {
            range: range,
            oldRange: oldRange,
            source: source,
            format: format
        }).catch(err => console.error('Error in selection-change:', err));
    });

    editorStates.set(editorId, { quill, dotNetRef, textChangeTimeout });
}

/**
 * Disposes of an editor instance
 * @param {string} editorId - Unique identifier for the editor
 */
export function disposeEditor(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        clearTimeout(stored.textChangeTimeout);
        editorStates.delete(editorId);
    }
}

/**
 * Sets the HTML content of the editor
 * @param {string} editorId - Unique identifier for the editor
 * @param {string} html - HTML content to set
 */
export function setHtml(editorId, html) {
    const stored = editorStates.get(editorId);
    if (stored) {
        stored.quill.root.innerHTML = html || '';
    }
}

/**
 * Gets the HTML content of the editor
 * @param {string} editorId - Unique identifier for the editor
 * @returns {string} HTML content
 */
export function getHtml(editorId) {
    const stored = editorStates.get(editorId);
    if (stored) {
        return stored.quill.root.innerHTML;
    }
    return '';
}

/**
 * Sets the editor contents using a Delta object
 * @param {string} editorId - Unique identifier for the editor
 * @param {string} delta - JSON string representation of the Delta
 */
export function setContents(editorId, delta) {
    const stored = editorStates.get(editorId);
    if (stored && delta) {
        stored.quill.setContents(JSON.parse(delta));
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

        // Special handling for code-block removal in Quill v2
        // TODO: Quill's format('code-block', false) doesn't work correctly - see DEV-TASKS.md #32
        if (formatName === 'code-block' && value === false && range) {
            // For now, just call format - removal doesn't work but at least won't error
            quill.format(formatName, value);
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

/**
 * Prompts for a URL and inserts a link
 * @param {string} editorId - Unique identifier for the editor
 * @returns {boolean} True if link was inserted, false if cancelled
 */
export function promptLink(editorId) {
    const stored = editorStates.get(editorId);
    if (!stored) return false;

    const quill = stored.quill;
    const range = quill.getSelection();

    if (!range) {
        // No selection - can't insert link
        return false;
    }

    // Check if there's already a link
    const format = quill.getFormat(range);
    if (format.link) {
        // Remove existing link
        quill.format('link', false);
        return true;
    }

    // Prompt for URL
    const url = window.prompt('Enter URL:', 'https://');
    if (url && url !== 'https://') {
        quill.format('link', url);
        return true;
    }

    return false;
}

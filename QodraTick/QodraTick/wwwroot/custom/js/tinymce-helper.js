
window.destroyEditor = function (editorId) {
    const editor = tinymce.get(editorId);
    if (editor) {
        editor.destroy();
    }
};

window.initializeTinyMCE = function (editorId, dotNetRef, options) {
    tinymce.init({
        selector: `#${editorId}`,
        height: options.height,
        language: 'fa',
        directionality: 'rtl',
        plugins: [
            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
            'anchor', 'searchreplace', 'visualblocks', 'code', 'fullscreen',
            'insertdatetime', 'media', 'table', 'help', 'wordcount'
        ],
        toolbar: 'undo redo | blocks | ' +
            'bold italic underline forecolor backcolor | alignleft aligncenter ' +
            'alignright alignjustify | bullist numlist outdent indent | ' +
            'removeformat | help' +
            (options.allowImageUpload ? ' | image' : ''),

        content_style: 'body { font-family: Tahoma, Arial, sans-serif; font-size: 14px; direction: rtl; }',

        // Image upload as base64
        automatic_uploads: false,
        file_picker_types: 'image',
        file_picker_callback: function (cb, value, meta) {
            if (meta.filetype === 'image') {
                var input = document.createElement('input');
                input.setAttribute('type', 'file');
                input.setAttribute('accept', 'image/*');

                input.addEventListener('change', function (e) {
                    var file = e.target.files[0];
                    if (file) {
                        var reader = new FileReader();
                        reader.addEventListener('load', function () {
                            cb(reader.result, {
                                alt: file.name,
                                title: file.name
                            });
                        });
                        reader.readAsDataURL(file);
                    }
                });

                input.click();
            }
        },

        // Set initial content
        init_instance_callback: function (editor) {
            if (options.initialValue) {
                editor.setContent(options.initialValue);
            }
        },

        // Handle content changes
        setup: function (editor) {
            editor.on('change keyup', function () {
                var content = editor.getContent();
                dotNetRef.invokeMethodAsync('OnContentChanged', content);
            });
        }
    });
};

window.setEditorContent = function (editorId, content) {
    const editor = tinymce.get(editorId);
    if (editor) {
        editor.setContent(content);
    }
};

window.getEditorContent = function (editorId) {
    const editor = tinymce.get(editorId);
    return editor ? editor.getContent() : '';
};



// TinyMCE Helper Functions
let editorInstances = {};

window.initializeTinyMCE = (editorId, dotNetRef, options) => {
    try {
        tinymce.init({
            selector: `#${editorId}`,
            height: options.height || 300,
            menubar: false,
            plugins: [
                'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
                'anchor', 'searchreplace', 'visualblocks', 'code', 'fullscreen',
                'insertdatetime', 'media', 'table', 'help', 'wordcount', 'paste'
            ],
            toolbar: 'undo redo | blocks | ' +
                'bold italic forecolor | alignleft aligncenter ' +
                'alignright alignjustify | bullist numlist outdent indent | ' +
                'removeformat | help',
            content_style: 'body { font-family: Tahoma, Arial, sans-serif; font-size:14px; direction: rtl; }',
            directionality: 'rtl',
            language: 'fa',
            paste_data_images: options.allowImageUpload || false,
            images_upload_handler: function (blobInfo, success, failure) {
                if (options.allowImageUpload) {
                    // Convert blob to base64
                    const reader = new FileReader();
                    reader.onload = function () {
                        success(reader.result);
                    };
                    reader.readAsDataURL(blobInfo.blob());
                } else {
                    failure('Image upload is not allowed');
                }
            },
            setup: function (editor) {
                editorInstances[editorId] = editor;

                // Set initial content
                if (options.initialValue) {
                    editor.on('init', function () {
                        editor.setContent(options.initialValue);
                    });
                }

                // Listen for content changes
                editor.on('change input paste', function () {
                    const content = editor.getContent();
                    dotNetRef.invokeMethodAsync('OnContentChanged', content);
                });
            }
        });
    } catch (error) {
        console.error('Error initializing TinyMCE:', error);
    }
};

window.destroyEditor = (editorId) => {
    try {
        if (editorInstances[editorId]) {
            editorInstances[editorId].destroy();
            delete editorInstances[editorId];
        }
    } catch (error) {
        console.error('Error destroying TinyMCE:', error);
    }
};
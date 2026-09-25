window.photoAlbum = {
    createObjectUrl: function (contentType, bytes) {
        const blob = new Blob([bytes], { type: contentType });
        return URL.createObjectURL(blob);
    },
    revokeObjectUrl: function (url) {
        if (url) {
            URL.revokeObjectURL(url);
        }
    }
};

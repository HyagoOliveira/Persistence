mergeInto(LibraryManager.library, {
    // Flushes our file changes to Browser IndexedDB.
    SyncDB: function () {
        FS.syncfs(false, function (error) {
           if (error) console.log("Syncfs error: " + error);
        });
    }
});
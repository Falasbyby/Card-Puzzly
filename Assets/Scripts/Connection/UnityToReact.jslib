var UnityToReactPlugin = {
    UnityToReact: function(message)
    {
        window.dispatchReactUnityEvent(
            "UnityToReact",
            UTF8ToString(message)
        );
    }
};

mergeInto(LibraryManager.library, UnityToReactPlugin);
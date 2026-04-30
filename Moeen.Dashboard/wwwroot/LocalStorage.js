<script>
    window.storage = {
        set: (key, value) => localStorage.setItem(key, value),
        get: (key) => localStorage.getItem(key),
        remove: (key) => localStorage.removeItem(key)
    };
</script>
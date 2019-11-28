(() => {
    $("#edit-legend").on("click", () => {
        $("#legend-show-block").css("display", "none");
        $("#legend-edit-block").css("display", "inline-flex");

        $(".events").each((_, item) => $(item).children().remove());
    });

    $("#save-legend").on("click", () => {
        const hashes = $("#selector").selectize().val();

        document.location.href = `/${hashes}`;
    });
    
    const selector = $("#selector"); //TODO: only on edit pressed
    selector.selectize({
        valueField: "hash",
        labelField: "title",
        searchField: "html",
        // maxOptions: 10,
        maxItems: 12,
        create: false,
        // items: initialItems.map(item => item.hash),
        // options: initialItems,
        score: (item) => (item2) => 1,
        render: {
            option: function(item, escape) {
                return `<div>${item.html}</div>`;
            }
        },
        load: function(query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: "http://localhost:65222/api/Search?query=" + encodeURIComponent(query),
                type: "GET",
                error: function() {
                    callback();
                },
                success: function(res) {
                    $("#selector").selectize()[0].selectize.clearOptions();
                    callback(res);
                }
            });
        }
    });
})();


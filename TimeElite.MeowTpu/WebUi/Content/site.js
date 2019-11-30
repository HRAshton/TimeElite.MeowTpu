(() => {
    $("#clear-legend").on("click", () => {
        $("#legend-show-block").css("display", "none");
        $("#legend-edit-block").css("display", "inline-flex");

        $(".events").each((_, item) => $(item).children().remove());
        document.cookie = "link=; expires=Thu, 01 Jan 1970 00:00:01 GMT;";
    });

    $("form").on("submit",
        (e) => {
            const hashes = $("#selector").selectize().val();
            const link = `0;0;${hashes};`;

            document.location.href = `/?${link}`;
            
            e.preventDefault();
            return false;
        });

    $("#save-legend").on("click", () => {
        $("form").submit();
    });
     
    $(".event").each((index, item) => {
        const hideId = $(item).data("hideid");
        const currentLink = window.location.href;

        $(item).on(
            "click",
            e => {
                if (currentLink.includes(hideId)) {
                    window.location.href = window.location.href
                        .replace(hideId, "")
                        .replace(",,", ",");
                } else {
                    window.location.href += hideId + ",";
                }
            });
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
                url: `${window.location.origin}/api/Search?query=${encodeURIComponent(query)}`,
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


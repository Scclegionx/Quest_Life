// Phone-only behaviour: dashboard task tabs, the floating "+" button, and the heat map starting at "today".
(function () {
    // ----- dashboard: show one task list at a time on small screens -----
    var tabs = document.querySelectorAll('.task-tab');
    var cols = document.querySelectorAll('.task-col-wrap');
    var fab = document.getElementById('fab');
    var types = { habits: 'Habit', dailies: 'Daily', todos: 'Todo' };

    function show(name) {
        tabs.forEach(function (t) { t.classList.toggle('active', t.dataset.tab === name); });
        cols.forEach(function (c) { c.classList.toggle('is-active', c.dataset.col === name); });
        if (fab) fab.href = fab.href.replace(/type=[A-Za-z]+/, 'type=' + types[name]);
        try { localStorage.setItem('ql-task-tab', name); } catch (e) { }
    }

    if (tabs.length) {
        tabs.forEach(function (t) { t.addEventListener('click', function () { show(t.dataset.tab); }); });
        var saved = null;
        try { saved = localStorage.getItem('ql-task-tab'); } catch (e) { }
        show(saved && types[saved] ? saved : 'todos');
    }

    // ----- heat map: newest weeks are on the right, so start scrolled to the end -----
    document.querySelectorAll('.heatmap-scroll').forEach(function (el) { el.scrollLeft = el.scrollWidth; });

    // ----- shop tab rows: keep the selected tab in view -----
    document.querySelectorAll('.tab-row').forEach(function (row) {
        var active = row.querySelector('a.active');
        if (active && row.scrollWidth > row.clientWidth) row.scrollLeft = active.offsetLeft - 16;
    });
})();

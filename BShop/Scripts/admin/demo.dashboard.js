!(function (o) {
  "use strict";
  function e() {
    (this.$body = o("body"));
  }
  (e.prototype.init = function () {
      o("#dash-daterange").daterangepicker({ singleDatePicker: !0 })
    }),
    (o.Dashboard = new e()),
    (o.Dashboard.Constructor = e);
})(window.jQuery),
  (function (t) {
    "use strict";
    t(document).ready(function (e) {
      t.Dashboard.init();
    });
  })(window.jQuery);

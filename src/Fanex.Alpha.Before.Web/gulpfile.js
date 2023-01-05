// Sass configuration
var gulp = require("gulp");
var sass = require("gulp-sass")(require("sass"));

gulp.task("sass", function (cb) {
  gulp
    .src([
      "wwwroot/assets/styles/vendors-extensions/login/bootstrap-ex.scss",
      "wwwroot/assets/styles/vendors/bootstrap.scss",
      "wwwroot/assets/styles/themes/*.scss",
      "wwwroot/assets/styles/pages/*.scss",
    ])
    .pipe(sass())
    .pipe(
      gulp.dest(function (f) {
        return f.base;
      })
    );
  cb();
});

gulp.task(
  "default",
  gulp.series("sass", function (cb) {
    gulp.watch("*.scss", gulp.series("sass"));
    cb();
  })
);

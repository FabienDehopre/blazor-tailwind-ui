const gulp = require('gulp');

gulp.task('css', () => {
  const postcss = require('gulp-postcss');
  return gulp.src('./Styles/styles.css')
    .pipe(postcss([
      require('postcss-import'),
      require('tailwindcss'),
      require('postcss-preset-env')({ stage: 1, autoprefixer: { grid: true } }),
    ]))
    .pipe(gulp.dest('./wwwroot/css/'));
});
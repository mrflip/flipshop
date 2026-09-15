
// lodash
/** Creates a compiled template function that can interpolate data properties
 * in "interpolate" delimiters, HTML-escape interpolated data properties in
 * "escape" delimiters, and execute JavaScript in "evaluate" delimiters. Data
 * properties may be accessed as free variables in the template. If a setting
 * object is given, it takes precedence over `templateSettings` values.
 *
 * **Security:** `template` is insecure and should not be used. It will be
 * removed in Lodash v5. Avoid untrusted input. See
 * [threat model](https://github.com/lodash/lodash/blob/main/threat-model.md).
 *
 * **Note:** In the development build `template` utilizes
 * [sourceURLs](http://www.html5rocks.com/en/tutorials/developertools/sourcemaps/#toc-sourceurl)
 * for easier debugging.
 *
 * For more information on precompiling templates see
 * [lodash's custom builds documentation](https://lodash.com/custom-builds).
 *
 * For more information on Chrome extension sandboxes see
 * [Chrome's extensions documentation](https://developer.chrome.com/extensions/sandboxingEval).
 *
 * @param string {string}: The template string; defaults to `''`.
 *   @optional
 * @param options {{
 *    @field escape {RegExp}: The HTML "escape" delimiter; defaults to `templateSettings.escape`.
 *     @optional
 *    @field evaluate {RegExp}: The "evaluate" delimiter; defaults to `templateSettings.evaluate`.
 *     @optional
 *    @field imports {map}: An object to import into the template as free variables; defaults to `templateSettings.imports`.
 *     @optional
 *    @field interpolate {RegExp}: The "interpolate" delimiter; defaults to `templateSettings.interpolate`.
 *     @optional
 *    @field sourceURL {string}: '] The sourceURL of the compiled template; defaults to `'lodash.templateSources[n`.
 *     @optional
 *    @field variable {string}: The data object variable name; defaults to `'obj'`.
 *     @optional
 * }}
 *
 * @returns {function}: the compiled template function.
 *
 * @example `var compiled = template('hello <%= user %>!'); compiled({ 'user': 'fred' }); // => 'hello fred!'`
 * @example `var compiled = template('<b><%- value %></b>'); compiled({ 'value': '<script>' }); // => '<b>&lt;script&gt;</b>'`
 * @example `var compiled = template('<% forEach(users, function(user) { %><li><%- user %></li><% }); %>'); compiled({ 'users': ['fred', 'barney'] }); // => '<li>fred</li><li>barney</li>'`
 * @example `var compiled = template('<% print("hello " + user); %>!'); compiled({ 'user': 'barney' }); // => 'hello barney!'`
 * @example `var compiled = template('hello ${ user }!'); compiled({ 'user': 'pebbles' }); // => 'hello pebbles!'`
 * @example `var compiled = template('<%= "\\<%- value %\\>" %>'); compiled({ 'value': 'ignored' }); // => '<%- value %>'`
 * @example `var text = '<% jq.each(users, function(user) { %><li><%- user %></li><% }); %>'; var compiled = template(text, { 'imports': { 'jq': jQuery } }); compiled({ 'users': ['fred', 'barney'] }); // => '<li>fred</li><li>barney</li>'`
 * @example `var compiled = template('hello <%= user %>!', { 'sourceURL': '/basic/greeting.jst' }); compiled(data); // => Find the source of "greeting.jst" under the Sources tab or Resources panel of the web inspector.`
 * @example `var compiled = template('hi <%= data.user %>!', { 'variable': 'data' }); compiled.source; // => function(data) {`
 * @example `templateSettings.interpolate = /{{([\s\S]+?)}}/g; var compiled = template('hello {{ user }}!'); compiled({ 'user': 'mustache' }); // => 'hello mustache!'`
 * @example `fs.writeFileSync(path.join(process.cwd(), 'jst.js'), '\ var JST = {\ "main": ' + template(mainText).source + '\ };\ ');`
 */
export function template(string, options) {
  if (false) { template(string, options); }
  throw 'TODO: implement template';
}
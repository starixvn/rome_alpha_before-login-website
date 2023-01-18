/* global Detecas */

var Detecas = Detecas || {};

Detecas.JSON = (function (window) {
    "use strict";

    // JSON2: https://github.com/douglascrockford/JSON-js/blob/master/json2.js
    // source: https://cdnjs.cloudflare.com/ajax/libs/json2/20150503/json2.min.js
    /* jshint ignore:start */
    "object" != typeof JSON && (JSON = {}), function () { "use strict"; function f(t) { return 10 > t ? "0" + t : t } function this_value() { return this.valueOf() } function quote(t) { return rx_escapable.lastIndex = 0, rx_escapable.test(t) ? '"' + t.replace(rx_escapable, function (t) { var e = meta[t]; return "string" == typeof e ? e : "\\u" + ("0000" + t.charCodeAt(0).toString(16)).slice(-4) }) + '"' : '"' + t + '"' } function str(t, e) { var r, n, o, u, f, a = gap, i = e[t]; switch (i && "object" == typeof i && "function" == typeof i.toJSON && (i = i.toJSON(t)), "function" == typeof rep && (i = rep.call(e, t, i)), typeof i) { case "string": return quote(i); case "number": return isFinite(i) ? i + "" : "null"; case "boolean": case "null": return i + ""; case "object": if (!i) return "null"; if (gap += indent, f = [], "[object Array]" === Object.prototype.toString.apply(i)) { for (u = i.length, r = 0; u > r; r += 1) f[r] = str(r, i) || "null"; return o = 0 === f.length ? "[]" : gap ? "[\n" + gap + f.join(",\n" + gap) + "\n" + a + "]" : "[" + f.join(",") + "]", gap = a, o } if (rep && "object" == typeof rep) for (u = rep.length, r = 0; u > r; r += 1) "string" == typeof rep[r] && (n = rep[r], o = str(n, i), o && f.push(quote(n) + (gap ? ": " : ":") + o)); else for (n in i) Object.prototype.hasOwnProperty.call(i, n) && (o = str(n, i), o && f.push(quote(n) + (gap ? ": " : ":") + o)); return o = 0 === f.length ? "{}" : gap ? "{\n" + gap + f.join(",\n" + gap) + "\n" + a + "}" : "{" + f.join(",") + "}", gap = a, o } } var rx_one = /^[\],:{}\s]*$/, rx_two = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, rx_three = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, rx_four = /(?:^|:|,)(?:\s*\[)+/g, rx_escapable = /[\\\"\u0000-\u001f\u007f-\u009f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, rx_dangerous = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g; "function" != typeof Date.prototype.toJSON && (Date.prototype.toJSON = function () { return isFinite(this.valueOf()) ? this.getUTCFullYear() + "-" + f(this.getUTCMonth() + 1) + "-" + f(this.getUTCDate()) + "T" + f(this.getUTCHours()) + ":" + f(this.getUTCMinutes()) + ":" + f(this.getUTCSeconds()) + "Z" : null }, Boolean.prototype.toJSON = this_value, Number.prototype.toJSON = this_value, String.prototype.toJSON = this_value); var gap, indent, meta, rep; "function" != typeof JSON.stringify && (meta = { "\b": "\\b", "	": "\\t", "\n": "\\n", "\f": "\\f", "\r": "\\r", '"': '\\"', "\\": "\\\\" }, JSON.stringify = function (t, e, r) { var n; if (gap = "", indent = "", "number" == typeof r) for (n = 0; r > n; n += 1) indent += " "; else "string" == typeof r && (indent = r); if (rep = e, e && "function" != typeof e && ("object" != typeof e || "number" != typeof e.length)) throw Error("JSON.stringify"); return str("", { "": t }) }), "function" != typeof JSON.parse && (JSON.parse = function (text, reviver) { function walk(t, e) { var r, n, o = t[e]; if (o && "object" == typeof o) for (r in o) Object.prototype.hasOwnProperty.call(o, r) && (n = walk(o, r), void 0 !== n ? o[r] = n : delete o[r]); return reviver.call(t, e, o) } var j; if (text += "", rx_dangerous.lastIndex = 0, rx_dangerous.test(text) && (text = text.replace(rx_dangerous, function (t) { return "\\u" + ("0000" + t.charCodeAt(0).toString(16)).slice(-4) })), rx_one.test(text.replace(rx_two, "@").replace(rx_three, "]").replace(rx_four, ""))) return j = eval("(" + text + ")"), "function" == typeof reviver ? walk({ "": j }, "") : j; throw new SyntaxError("JSON.parse") }) } ();
    /* jshint ignore:end */

    var defaultMaxDepth = 6;
    var defaultArrayMaxLength = 50;
    var seen; // Same variable used for all stringifications
    var stripFuncs;

    /*jshint -W121 */
    Date.prototype.toPrunedJSON = Date.prototype.toJSON;
    String.prototype.toPrunedJSON = String.prototype.toJSON;
    /*jshint +W121 */

    var escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        meta = { // table of character substitutions
            '\b': "\\b",
            '\t': "\\t",
            '\n': "\\n",
            '\f': "\\f",
            '\r': "\\r",
            '"': "\\\"",
            '\\': "\\\\"
        };

    function quote(string) {
        escapable.lastIndex = 0;
        return escapable.test(string) ? "\"" + string.replace(escapable, function (a) {
            var c = meta[a];
            return typeof c === "string" ? c : "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4);
        }) + "\"" : "\"" + string + "\"";
    }

    function str(key, holder, depthDecr, arrayMaxLength) {
        var i, k, v, length, partial, value = holder[key];
        if (value && typeof value === "object" && typeof value.toPrunedJSON === "function") {
            value = value.toPrunedJSON(key);
        }

        switch (typeof value) {
            case "string":
                return quote(value);
            case "number":
                return isFinite(value) ? String(value) : "null";
            case "boolean":
            case "null":
                return String(value);
            case "object":

                if (!value) {
                    return "null";
                }

                if (depthDecr <= 0 || seen.indexOf(value) !== -1) {
                    return "\"{*}\""; // pruned
                }

                seen.push(value);

                partial = [];

                if (Object.prototype.toString.apply(value) === "[object Array]") {
                    length = Math.min(value.length, arrayMaxLength);
                    for (i = 0; i < length; i += 1) {
                        partial[i] = str(i, value, depthDecr - 1, arrayMaxLength) || "null";
                    }
                    return partial.length ? "[" + partial.join(",") + "]" : "[]";
                }

                // Sort key first
                var keys = [];

                for (k in value) {
                    // Key must be a valid name
                    try {
                        if (value[k] && k.match(/^[a-zA-Z0-9]+$/)) {
                            keys.push(k);
                        }
                    } catch (e) { }
                }

                keys.sort();

                var c = keys.length;
                for (i = 0; i < c; i++) {
                    k = keys[i];
                    try {
                        v = str(k, value, depthDecr - 1, arrayMaxLength);
                        if (typeof v === "string") {
                            v = v.replace(/[^\u0000-\u007F]/g, ""); // Remove non-ascii characters
                            if (typeof stripFuncs[k] === "function") { // There is a inject function to remove a portion of string
                                partial.push(quote(k) + ":" + stripFuncs[k].call(JSON, v));
                            } else {
                                partial.push(quote(k) + ":" + v);
                            }
                        }
                    } catch (e) {
                        // This try/catch due to forbidden accessors on some objects
                    }
                }

                return partial.length ? "{" + partial.join(",") + "}" : "{}";
        }
    }

    var JSON = JSON || window.JSON;
    JSON.prune = function (value, option) {
        // depthDecr, arrayMaxLength, stripFuncs
        seen = [];
        option = option || {};
        option.depthDecr = option.depthDecr || defaultMaxDepth;
        option.arrayMaxLength = option.arrayMaxLength || defaultArrayMaxLength;
        stripFuncs = option.stripFuncs || {};
        return str("", {
            '': value
        }, option.depthDecr, option.arrayMaxLength);
    };

    return JSON;
})(window);
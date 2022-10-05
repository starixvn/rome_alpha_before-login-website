/* global Detecas */
/*jshint bitwise: false */
/*jslint eqeq: true*/

var Detecas = Detecas || {};

Detecas.Base64 = (function () {
    "use strict";

    var Base = (function () {
        function F() { }

        return {
            extend: function (overrides) {
                // Spawn
                F.prototype = this;
                var subtype = new F();

                // Augment
                if (overrides) {
                    subtype.mixIn(overrides);
                }

                // Create default initializer
                if (!subtype.hasOwnProperty('init')) {
                    subtype.init = function () {
                        subtype.$super.init.apply(this, arguments);
                    };
                }

                // Initializer's prototype is the subtype object
                subtype.init.prototype = subtype;

                // Reference supertype
                subtype.$super = this;

                return subtype;
            },
            create: function () {
                var instance = this.extend();
                instance.init.apply(instance, arguments);

                return instance;
            },
            init: function () {
            },
            mixIn: function (properties) {
                for (var propertyName in properties) {
                    if (properties.hasOwnProperty(propertyName)) {
                        this[propertyName] = properties[propertyName];
                    }
                }

                // IE won't copy toString using the loop above
                if (properties.hasOwnProperty('toString')) {
                    this.toString = properties.toString;
                }
            },
            clone: function () {
                return this.init.prototype.extend(this);
            }
        };
    } ());

    var WordArray = Base.extend({
        init: function (words, sigBytes) {
            words = this.words = words || [];

            if (sigBytes !== undefined) {
                this.sigBytes = sigBytes;
            } else {
                this.sigBytes = words.length * 4;
            }
        },
        toString: function (encoder) {
            return (encoder || Utf8).stringify(this);
        },
        concat: function (wordArray) {
            // Shortcuts
            var thisWords = this.words;
            var thatWords = wordArray.words;
            var thisSigBytes = this.sigBytes;
            var thatSigBytes = wordArray.sigBytes;

            // Clamp excess bits
            this.clamp();

            // Concat
            var i;
            if (thisSigBytes % 4) {
                // Copy one byte at a time
                for (i = 0; i < thatSigBytes; i++) {
                    var thatByte = (thatWords[i >>> 2] >>> (24 - (i % 4) * 8)) & 0xff;
                    thisWords[(thisSigBytes + i) >>> 2] |= thatByte << (24 - ((thisSigBytes + i) % 4) * 8);
                }
            } else if (thatWords.length > 0xffff) {
                // Copy one word at a time
                for (i = 0; i < thatSigBytes; i += 4) {
                    thisWords[(thisSigBytes + i) >>> 2] = thatWords[i >>> 2];
                }
            } else {
                // Copy all words at once
                thisWords.push.apply(thisWords, thatWords);
            }
            this.sigBytes += thatSigBytes;

            // Chainable
            return this;
        },
        clamp: function () {
            // Shortcuts
            var words = this.words;
            var sigBytes = this.sigBytes;

            // Clamp
            words[sigBytes >>> 2] &= 0xffffffff << (32 - (sigBytes % 4) * 8);
            words.length = Math.ceil(sigBytes / 4);
        },
        clone: function () {
            var clone = Base.clone.call(this);
            clone.words = this.words.slice(0);

            return clone;
        },
        random: function (nBytes) {
            var words = [];
            for (var i = 0; i < nBytes; i += 4) {
                words.push((Math.random() * 0x100000000) | 0);
            }

            return new WordArray.init(words, nBytes);
        }
    });

    var Latin1 = {
        stringify: function (wordArray) {
            // Shortcuts
            var words = wordArray.words;
            var sigBytes = wordArray.sigBytes;

            // Convert
            var latin1Chars = [];
            for (var i = 0; i < sigBytes; i++) {
                var bite = (words[i >>> 2] >>> (24 - (i % 4) * 8)) & 0xff;
                latin1Chars.push(String.fromCharCode(bite));
            }

            return latin1Chars.join('');
        },
        parse: function (latin1Str) {
            // Shortcut
            var latin1StrLength = latin1Str.length;

            // Convert
            var words = [];
            for (var i = 0; i < latin1StrLength; i++) {
                words[i >>> 2] |= (latin1Str.charCodeAt(i) & 0xff) << (24 - (i % 4) * 8);
            }

            return new WordArray.init(words, latin1StrLength);
        }
    };

    var Utf8 = {
        stringify: function (wordArray) {
            try {
                return decodeURIComponent(escape(Latin1.stringify(wordArray)));
            } catch (e) {
                throw new Error('Malformed UTF-8 data');
            }
        },
        parse: function (utf8Str) {
            return Latin1.parse(unescape(encodeURIComponent(utf8Str)));
        }
    };

    // refer: https://code.google.com/p/crypto-js/source/browse/tags/3.1.2/src/enc-base64.js
    var _map = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';

    function encode(input) {
        var wordArray = Utf8.parse(input);

        // Shortcuts
        var words = wordArray.words;
        var sigBytes = wordArray.sigBytes;
        var map = _map;

        // Clamp excess bits
        wordArray.clamp();

        // Convert
        var base64Chars = [];
        for (var i = 0; i < sigBytes; i += 3) {
            var byte1 = (words[i >>> 2] >>> (24 - (i % 4) * 8)) & 0xff;
            var byte2 = (words[(i + 1) >>> 2] >>> (24 - ((i + 1) % 4) * 8)) & 0xff;
            var byte3 = (words[(i + 2) >>> 2] >>> (24 - ((i + 2) % 4) * 8)) & 0xff;

            var triplet = (byte1 << 16) | (byte2 << 8) | byte3;

            for (var j = 0; (j < 4) && (i + j * 0.75 < sigBytes); j++) {
                base64Chars.push(map.charAt((triplet >>> (6 * (3 - j))) & 0x3f));
            }
        }

        // Add padding
        var paddingChar = map.charAt(64);
        if (paddingChar) {
            while (base64Chars.length % 4) {
                base64Chars.push(paddingChar);
            }
        }

        return base64Chars.join('');
    }

    function decode(base64Str) {
        // Shortcuts
        var base64StrLength = base64Str.length;
        var map = _map;

        // Ignore padding
        var paddingChar = map.charAt(64);
        if (paddingChar) {
            var paddingIndex = base64Str.indexOf(paddingChar);
            if (paddingIndex !== -1) {
                base64StrLength = paddingIndex;
            }
        }

        // Convert
        var words = [];
        var nBytes = 0;
        for (var i = 0; i < base64StrLength; i++) {
            if (i % 4) {
                var bits1 = map.indexOf(base64Str.charAt(i - 1)) << ((i % 4) * 2);
                var bits2 = map.indexOf(base64Str.charAt(i)) >>> (6 - (i % 4) * 2);
                words[nBytes >>> 2] |= (bits1 | bits2) << (24 - (nBytes % 4) * 8);
                nBytes++;
            }
        }

        var wordArray = WordArray.create(words, nBytes);
        return wordArray.toString(Utf8);
    }

    return {
        encode: encode,
        decode: decode
    };
})();
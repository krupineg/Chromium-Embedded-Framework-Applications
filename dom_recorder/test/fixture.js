var assert = require('assert');
var app = require('../src/app.js');

describe('Test Suite 1', function () {
    it('Test 1', function () {
        app.document.onload(function() {

        });
        
        assert.ok(true, "This shouldn't fail");
    });

    it('Test 2', function () {
        assert.ok(1 === 1, "This shouldn't fail");
        assert.ok(false, "This should fail");
    });
});

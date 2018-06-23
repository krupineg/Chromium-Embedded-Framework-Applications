"use strict";
function mutationCallback(name, obj) {
    console.log(name);
    console.log(obj);
}

module.exports = { mutationCallback };
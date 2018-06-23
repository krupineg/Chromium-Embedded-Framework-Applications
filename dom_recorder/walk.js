let dom = function (node, func) {
    if (node) {
        func(node);                     //What does this do?
        node = node.firstChild;
        while (node) {
            dom(node, func);
            node = node.nextSibling;
        }
    }
};

module.exports = { dom };
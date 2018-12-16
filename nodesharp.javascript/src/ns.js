const NodeSharpFunction = require('./nsFunction');

var funcDict = {};

module.exports.resolve = function(func) {
    var nsFunc = funcDict[func.name];
    var result;
    result = nsFunc.invoke(func.arguements);
    return result;
}

module.exports.bind = function(func, optionsPredicate) {
    var nsFunc = new NodeSharpFunction(func);
    optionsPredicate(nsFunc);
    funcDict[func.prototype.constructor.name] = nsFunc;
}
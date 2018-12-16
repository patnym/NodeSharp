

class NodeSharpFunction {

    constructor(func) {
        this.arguements = [];
        this.returns = null;
        this.func = func;
    }

    invoke(arguementList) {
        if(arguementList.length != this.arguements.length) {
            throw new Error("Arguement list size must match the function arguemetn list size");
        }
        return this.func.apply(null, arguementList);
    }
}

module.exports = NodeSharpFunction;
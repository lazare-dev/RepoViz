// api/pkg/analyzer/parseJS.js

const fs = require('fs');
const path = require('path');
const { parse } = require('@babel/parser');
const traverse = require('@babel/traverse').default;

const graph = { nodes: [], edges: [] };

function addNode(id, type, label = '', filePath) {
    const existingNode = graph.nodes.find(node => node.id === id);
    if (!existingNode) {
        const normalizedPath = filePath.replace(/\\/g, '/');
        graph.nodes.push({ id, type, label, filePath: normalizedPath, language: 'JavaScript' });
    }
}

function addEdge(source, target, label) {
    const edgeId = `${source}_${target}`;
    const existingEdge = graph.edges.find(edge => `${edge.source}_${edge.target}` === edgeId);
    if (!existingEdge && source && target) {
        graph.edges.push({ source, target, label });
    }
}

function processFile(filePath) {
    const content = fs.readFileSync(filePath, 'utf8');
    const ast = parse(content, {
        sourceType: 'module',
        plugins: ['jsx', 'classProperties', 'decorators-legacy', 'typescript']
    });

    const fileNodeID = path.basename(filePath, path.extname(filePath)); // Ensures fileNodeID is unique without extension
    addNode(fileNodeID, 'file', '', filePath);

    traverse(ast, {
        ImportDeclaration: ({ node }) => {
            const sourceID = `import:${node.source.value}:${fileNodeID}`;
            addNode(sourceID, 'import', node.source.value, filePath);
            addEdge(fileNodeID, sourceID, 'imports');
        },
        FunctionDeclaration: ({ node }) => {
            const funcName = node.id ? node.id.name : 'Anonymous';
            const funcID = `function:${funcName}:${fileNodeID}`;
            addNode(funcID, 'function', funcName, filePath);
            addEdge(fileNodeID, funcID, 'declares');
        },
        CallExpression: ({ node }) => {
            let calleeName = '';
            if (node.callee.type === 'Identifier') {
                calleeName = node.callee.name;
            } else if (node.callee.type === 'MemberExpression' && node.callee.property.type === 'Identifier') {
                calleeName = node.callee.property.name;
            }
            if (calleeName) {
                const callID = `call:${calleeName}:${fileNodeID}`;
                addNode(callID, 'call', calleeName, filePath);
                addEdge(fileNodeID, callID, 'calls');
            }
        },
        ClassDeclaration: ({ node }) => {
            const className = node.id.name;
            const classID = `class:${className}:${fileNodeID}`;
            addNode(classID, 'class', className, filePath);
            addEdge(fileNodeID, classID, 'declares');
        }
    });
}

function parseJSFiles(directory) {
    fs.readdirSync(directory).forEach(file => {
        const fullPath = path.join(directory, file);
        if (file.endsWith('.js') || file.endsWith('.jsx')) {
            processFile(fullPath);
        }
    });

    console.log(JSON.stringify(graph, null, 2));
}

const directory = process.argv[2];
if (!directory) {
    console.error('Directory path not provided.');
    process.exit(1);
}
parseJSFiles(directory);

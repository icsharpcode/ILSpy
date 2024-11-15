module.exports = {
    'env': {
        'commonjs': true,
        'es6': true,
        'browser': true
    },
    'extends': 'eslint:recommended',
    'parserOptions': {
        'sourceType': 'module',
        'ecmaVersion': 'latest'
    },
    'rules': {
        'indent': ['error', 4, { 'SwitchCase': 1 }],
        'semi': ['error', 'always']
    }
};
module.exports = {
    mode: 'development',
    entry: './src/app.js',
    devtool: 'nosources-source-map',

    output: {
        filename: 'dom-recorder.js',
        libraryTarget: 'var',
        library: 'domRecorder'
    }
};
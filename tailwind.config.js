module.exports = {
    content: [
        './_drafts/**/*.html',
        './_includes/**/*.html',
        './_layouts/**/*.html',
        './_posts/*.md',
        './*.md',
        './*.html',
    ],
    theme: {
        theme: {
            extend: {},
            fontFamily: {
                title: ['HelveticaDisplayBlack'],
                regular: ['HelveticaNow'],
                sans: [
                    "HelveticaNow",
                    "sans-serif"
                ],
                serif: [
                    "Roboto",
                    "Georgia",
                    "serif"
                ],
                mono: [
                    "Menlo",
                    "Monaco",
                    "Consolas",
                    "Liberation Mono",
                    "Courier New",
                    "monospace"
                ]
            },
        },
    },
    plugins: []
}
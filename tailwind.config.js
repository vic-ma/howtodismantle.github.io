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
        extend: {
            typography: ({ theme }) => ({
                dismantle: {
                    css: {
                        '--tw-prose-body': theme('colors.gray[200]'),
                        '--tw-prose-headings': theme('colors.white'),
                        '--tw-prose-lead': theme('colors.gray[300]'),
                        '--tw-prose-links': theme('colors.secondary'),
                        '--tw-prose-bold': theme('colors.white'),
                        '--tw-prose-counters': theme('colors.gray[400]'),
                        '--tw-prose-bullets': theme('colors.gray[600]'),
                        '--tw-prose-hr': theme('colors.gray[700]'),
                        '--tw-prose-quotes': theme('colors.gray[100]'),
                        '--tw-prose-quote-borders': theme('colors.gray[700]'),
                        '--tw-prose-captions': theme('colors.gray[400]'),
                        '--tw-prose-code': theme('colors.white'),
                        '--tw-prose-pre-code': theme('colors.gray[300]'),
                        '--tw-prose-pre-bg': 'rgb(0 0 0 / 50%)',
                        '--tw-prose-th-borders': theme('colors.gray[600]'),
                        '--tw-prose-td-borders': theme('colors.gray[700]'),
                        strong: {
                            'font-weight': 700,
                        },
                    },
                }
            }),
            colors: {
                blackground: '#1C1C1C',
                blueground: '#202C3C',
                primary: '#143761',
                secondary: '#21AFE1',
                light: '#ECEBE4',
            }
        },
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
    plugins: [
        require('@tailwindcss/forms'),
        require('@tailwindcss/typography')
    ],
}
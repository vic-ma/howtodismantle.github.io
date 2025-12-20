module.exports = {
    content: [
        './_drafts/**/*.html',
        './_includes/**/*.html',
        './_layouts/**/*.html',
        './_posts/*.md',
        './collections/**/*.html',
        './learning/**/*.html',
        './*.md',
        './*.html',
    ],
    darkMode: 'class',
    theme: {
        extend: {
            typography: ({ theme }) => ({
                dismantle: {
                    css: {
                        '--tw-prose-body': theme('colors.gray[800]'),
                        '--tw-prose-headings': theme('colors.gray[900]'),
                        '--tw-prose-lead': theme('colors.gray[700]'),
                        '--tw-prose-links': theme('colors.secondary'),
                        '--tw-prose-bold': theme('colors.gray[900]'),
                        '--tw-prose-counters': theme('colors.gray[600]'),
                        '--tw-prose-bullets': theme('colors.gray[400]'),
                        '--tw-prose-hr': theme('colors.gray[300]'),
                        '--tw-prose-quotes': theme('colors.gray[900]'),
                        '--tw-prose-quote-borders': theme('colors.gray[300]'),
                        '--tw-prose-captions': theme('colors.gray[700]'),
                        '--tw-prose-code': theme('colors.gray[900]'),
                        '--tw-prose-pre-code': theme('colors.gray[100]'),
                        '--tw-prose-pre-bg': theme('colors.gray[900]'),
                        '--tw-prose-th-borders': theme('colors.gray[300]'),
                        '--tw-prose-td-borders': theme('colors.gray[200]'),
                        '--tw-prose-invert-body': theme('colors.gray[200]'),
                        '--tw-prose-invert-headings': theme('colors.white'),
                        '--tw-prose-invert-lead': theme('colors.gray[300]'),
                        '--tw-prose-invert-links': theme('colors.secondary'),
                        '--tw-prose-invert-bold': theme('colors.white'),
                        '--tw-prose-invert-counters': theme('colors.gray[400]'),
                        '--tw-prose-invert-bullets': theme('colors.gray[600]'),
                        '--tw-prose-invert-hr': theme('colors.gray[700]'),
                        '--tw-prose-invert-quotes': theme('colors.gray[100]'),
                        '--tw-prose-invert-quote-borders': theme('colors.gray[700]'),
                        '--tw-prose-invert-captions': theme('colors.gray[400]'),
                        '--tw-prose-invert-code': theme('colors.white'),
                        '--tw-prose-invert-pre-code': theme('colors.gray[300]'),
                        '--tw-prose-invert-pre-bg': 'rgb(0 0 0 / 50%)',
                        '--tw-prose-invert-th-borders': theme('colors.gray[600]'),
                        '--tw-prose-invert-td-borders': theme('colors.gray[700]'),
                        strong: {
                            'font-weight': 700,
                        },
                        'code::before': {
                            content: '""'
                        },
                        'code::after': {
                            content: '""'
                        }
                    },
                }
            }),
            colors: {
                blackground: '#1C1C1C',
                blueground: '#202C3C',
                primary: '#143761',
                secondary: '#21AFE1',
                light: '#ECEBE4',
                'accent-cyan': '#2EC4FF',
                'accent-amber': '#FFB449',
            },
            keyframes: {
                highlight: {
                    '0%': { backgroundColor: 'transparent' },
                    '50%': { backgroundColor: 'rgb(33 175 225 / 0.1)' },
                    '100%': { backgroundColor: 'transparent' },
                },
            },
            animation: {
                highlight: 'highlight 2s ease-in-out',
            },
        },
        fontFamily: {
            title: ['HelveticaDisplayBlack'],
            regular: ['HelveticaNow'],
            sans: [
                "Inter",
                "HelveticaNow",
                "sans-serif"
            ],
            heading: [
                "Inter Tight",
                "Inter",
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
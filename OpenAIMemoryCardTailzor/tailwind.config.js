module.exports = {
	content: [
		"./**/*.html",
		"./**/*.razor",
		"./**/*.cshtml",
		"./**/appsettings.json",
		"./**/styles.css",
	],
	theme: {
		extend: {
			fontFamily: {
				'mono': ['Jetbrains Mono', 'Fira Code', 'Source Code Pro', 'Consolas', 'Roboto Mono', 'Courier New', 'mono'],
			},
			screens: {
				fhd: { max: "1919px" },
				xl2: { max: "1439px" },
				xl: { max: "1279px" },
				lg: { max: "1023px" },
				md: { max: "767px" },
				sm: { max: "639px" },
				xs: { max: "339px" },
			},
		},
	},

	plugins: [
		require("@tailwindcss/typography"),
		require("@tailwindcss/forms"),
		require("tailwind-scrollbar")
	],
}
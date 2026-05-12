import js from '@eslint/js';
import globals from 'globals';
import tseslint from 'typescript-eslint';
import prettierPlugin from 'eslint-plugin-prettier';
import sveltePlugin from 'eslint-plugin-svelte';
import svelteParser from 'svelte-eslint-parser';
import tsParser from '@typescript-eslint/parser';
import { defineConfig } from 'eslint/config';

export default defineConfig([
    {
        files: ['**/*.{js,mjs,cjs,ts}'],
        languageOptions: {
            sourceType: 'module',
            globals: {
                ...globals.browser
            }
        },
        plugins: {
            js,
            prettier: prettierPlugin
        },
        rules: {
            ...js.configs.recommended.rules,
            'prettier/prettier': 'error',
            '@typescript-eslint/no-unused-vars': 'off'
        }
    },
    ...tseslint.configs.recommended,
    {
        files: ['**/*.ts', '**/*.js'],
        rules: {
            '@typescript-eslint/no-unused-vars': 'off'
        }
    },
    ...sveltePlugin.configs['flat/recommended'],
    {
        files: ['**/*.svelte'],
        languageOptions: {
            parser: svelteParser,
            parserOptions: {
                parser: tsParser
            },
            globals: {
                ...globals.browser
            }
        },
        rules: {
            'prettier/prettier': 'off'
        }
    }
]);

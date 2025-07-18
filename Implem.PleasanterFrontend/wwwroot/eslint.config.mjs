import js from '@eslint/js';
import globals from 'globals';
import tseslint from 'typescript-eslint';
import prettierPlugin from 'eslint-plugin-prettier';
import { defineConfig } from 'eslint/config';

export default defineConfig([
    {
        files: ['**/*.{js,mjs,cjs,ts}'],
        languageOptions: {
            sourceType: 'commonjs',
            globals: {
                ...globals.browser,
                $p: 'readonly',
                $: 'readonly'
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
    }
]);

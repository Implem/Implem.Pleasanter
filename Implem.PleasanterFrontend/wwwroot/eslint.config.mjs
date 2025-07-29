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
                $: 'readonly',
                jQuery: 'readonly',
                marked: 'readonly',
                d3: 'readonly',
                md5: 'readonly',
                FullCalendar: 'readonly',
                GridStack: 'readonly',
                flatpickr: 'readonly',
                QRCode: 'readonly',
                $p: 'readonly',
                formId: 'writable',
                controlId: 'writable',
                $form: 'writable',
                $menuSort: 'writable'
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

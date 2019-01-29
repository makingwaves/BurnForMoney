import React from 'react';
import ReactDOM from 'react-dom';
import './i18n';

import { runWithAdal } from 'react-adal';
import { authContext } from './adalConfig';

import './style/style.css';
import App from './App';

runWithAdal(authContext, () => {
    ReactDOM.render(<App />, document.getElementById('root'));
}, true);

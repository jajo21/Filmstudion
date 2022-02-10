import { app } from '../script.js';

export function homePage() {
    app.mainContent.innerHTML = '';
    app.mainContent.insertAdjacentHTML('beforeend', `\
    <div id="home-main">
        Välkommen till SFF:s filmhyrar-sida!
    </div>`);
}

export function runHomePage() {
    app.home.addEventListener('click', function() {
        homePage();
    });
}
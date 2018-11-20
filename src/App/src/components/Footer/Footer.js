import React, { Component } from 'react';

import logoMW from 'img/mw-logo-black.svg';
import NoA_member from 'img/noa-member.svg';
import { withNamespaces } from 'react-i18next';
import './Footer.css';

class Footer extends Component {
  render() {
    const { t } = this.props;
    return (
      <div className="Footer">
        <div className="Footer__container container">
          <div className="Footer__logo">
            <img className="Footer__logo-image" src={logoMW} alt="" />
          </div>
          <div className="Footer__links">
            <ul className="Footer__links-list">
              <li className="Footer__links-listItem"><a href="http://makingwaves.com/">makingwaves.com</a></li>
              <li className="Footer__links-listItem"><a href="http://praca.makingwaves.com/#initiatives">{t('Our initiatives')}</a></li>
              <li className="Footer__links-listItem"><a href="https://www.makingwaves.com/contact-us/">{t('Contact us')}</a></li>
            </ul>
          </div>
          <div className="Footer__company">
            <p className="Footer__company-name">Making Waves Kraków</p>
            <p className="Footer__company-contact">
              <a href="mailto:">post-poland@makingwaves.com</a><br/>
              +48 12 357 20 50
            </p>
            <p className="Footer__company-address">
              Adama Asnyka 9<br/>
              31-144 Kraków<br/>
              Poland
            </p>
          </div>
          <div className="Footer__membership">
            <img className="Footer__membership-logo" src={NoA_member} alt="A member of NoA family"/>
          </div>
        </div>
      </div>
    );
  }
}

export default withNamespaces()(Footer);

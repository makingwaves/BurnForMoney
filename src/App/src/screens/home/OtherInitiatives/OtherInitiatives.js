import React, { Component } from 'react';
import { withNamespaces, Trans } from 'react-i18next';

import './OtherInitiatives.css';

class OtherInitiatives extends Component {
  render() {
    const { t } = this.props;
    return (
      <div className="OtherInitiatives">
        <div className="OtherInitiatives__container container">
          <h2 className="OtherInitiatives__header Header">{t('Other Initiatives')}</h2>
          <p className="OtherInitiatives__text"><Trans i18nKey="Conferences, employees support, events sponsorship and other charity programmes">Conferences, employees support, events sponsorship<br/> and other charity programmes</Trans></p>
          <a href="http://praca.makingwaves.com/#initiatives" className="OtherInitiatives__button">{t('See our initiatives')}</a>
        </div>
      </div>
    );
  }
}

export default withNamespaces()(OtherInitiatives);

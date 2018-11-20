import React, { Component } from 'react';

import './TotalNumbers.css';

import { withNamespaces, Trans } from 'react-i18next';

class TotalNumbers extends Component {
  render() {
    const { t } = this.props;

    return (
      <div className="TotalNumbers">
        <div className="TotalNumebrs__container container">

          <h2 className="TotalNumbers__header Header"><strong className="flames">Burn For Money</strong> <Trans i18nKey="is a CSR internal initiative that connects charity with fit lifestyle.">is <span class="no-break-space">a CSR</span> internal initiative that connects charity with fit lifestyle.</Trans></h2>
          <h4>{t('Our achievements so far')}</h4>
          <div className="TotalNumbers__equation">
            <div className="TotalNumbers__equation-circle">
              <div>
                <span className="TotalNumbers__equation-value">{(this.props.data ? this.props.data.distance : 0)} km</span><br/>
                {t('On route')}
              </div>
            </div>
            <div className="TotalNumbers__equation-operator">
              +
            </div>
            <div className="TotalNumbers__equation-circle">
              <div>
                <span className="TotalNumbers__equation-value">{(this.props.data ? this.props.data.time : 0)} h</span><br/>
                {t('Of training')}
              </div>
            </div>
            <div className="TotalNumbers__equation-operator">
              =
            </div>
            <div className="TotalNumbers__equation-circle">
              <div>
                <span className="TotalNumbers__equation-value">{(this.props.data ? this.props.data.money : 0)} PLN</span><br/>
                {t('Given to help')}
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default withNamespaces()(TotalNumbers);

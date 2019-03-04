import React, { Component } from 'react';

import './MonthStats.css';

import { withNamespaces } from 'react-i18next';

class MonthSummaryStats extends Component {

    render() {
        const { t } = this.props;
        return (
            <div className="MonthSummaryStats">
                <div className="MonthSummaryStats-cart">
                    <div className="MonthSummaryStats-cart-val">{(this.props.data ? this.props.data.thisMonth.numberOfTrainings : 0)}</div>
                    <div className="MonthSummaryStats-cart-txt">{t("TV_Training_sessions")}</div>
                </div>
                <div className="MonthSummaryStats-cart">
                    <div className="MonthSummaryStats-cart-val">{(this.props.data ? this.props.data.thisMonth.percentOfEngagedEmployees : 0)}%</div>
                    <div className="MonthSummaryStats-cart-txt">{t("TV_Making_Wavers_engaged")}</div>
                </div>
                <div className="MonthSummaryStats-cart MonthSummaryStats-cart-exposed">
                    <div className="MonthSummaryStats-cart-val">{(this.props.data ? this.props.data.thisMonth.money : 0)} zł</div>
                    <div className="MonthSummaryStats-cart-txt">{t("TV_Collected_this_month_so_far")}</div>
                </div>
            </div>
        );
    }
}

export default withNamespaces()(MonthSummaryStats);
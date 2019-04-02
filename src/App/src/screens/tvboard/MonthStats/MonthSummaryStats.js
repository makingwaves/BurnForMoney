import React, { Component } from 'react';

import './MonthStats.css';

import { withNamespaces } from 'react-i18next';

const MonthSummaryStatsCard = (props) => {
    return (
        <div className={"MonthSummaryStats-card " + (props.exposed ? "MonthSummaryStats-card-exposed" : "")}>
            <div className="MonthSummaryStats-card-val">{props.value}</div>
            <div className="MonthSummaryStats-card-txt">{props.text}</div> 
        </div>
    );
}

class MonthSummaryStats extends Component {

    render() {
        const { t } = this.props;
        return (
            <div className="MonthSummaryStats">
                <MonthSummaryStatsCard  
                    value={(this.props.data ? this.props.data.thisMonth.numberOfTrainings : 0)} 
                    text={t("TV_Training_sessions")}
                />

                <MonthSummaryStatsCard  
                    value={(this.props.data ? this.props.data.thisMonth.percentOfEngagedEmployees : 0) + "%"}
                    text={t("TV_Making_Wavers_engaged")}
                />

                <MonthSummaryStatsCard
                    exposed={true}
                    value={(this.props.data ? this.props.data.thisMonth.money : 0) + " zł"} 
                    text={t("TV_Collected_this_month_so_far")}
                />
            </div>
        );
    }
}

export default withNamespaces()(MonthSummaryStats);
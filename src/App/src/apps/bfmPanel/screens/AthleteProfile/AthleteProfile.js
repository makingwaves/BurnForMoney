import React, { Component } from 'react';

import './AthleteProfile.css';
import authFetch from "../../../../components/Authentication/AuthFetch"

class AthleteProfile extends Component {
  constructor(props) {
    super(props);
    this.state = {
      activities: []
    }
  }

  render() {
    return (
      <div className="AthleteProfile">
        HELLo:
        {this.state.activities.length > 0 && (
          <ul>
          {this.state.activities.map( (activity) =>
            <li key={activity.id}>{`${activity.activityType} (${activity.movingTimeInMinutes}min)`}</li>
          )}
          </ul>
        )}
      </div>
    );
  }
  componentDidMount(){
    // internal api_url
    const api_url = process.env.REACT_APP_DASHBOARD_API_URL;
    authFetch(api_url+"api/athletes/"+this.props.match.params.athleteId+"/activities")
      .then(res => res.json())
      .then(
        (result) => {console.log("RESULT:",result); this.setState({activities: result}); console.log('ACTIVITIES',this.state.activities); },
        (error) => {console.error('Error:', error); }
      );

  }


}
export default AthleteProfile;

// https://bfmfunc-internal-prod.azurewebsites.net/api/athlete/{athleteId:length(32)}/activities

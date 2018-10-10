import React, { Component } from 'react';

import './HowItWorks.css';

class HowItWorks extends Component {
  render() {
    return (
      <div className="HowItWorks">
        <div className="HowItWorks__container container">
          <h3 className="HowItWorks__header Header">How it works:</h3>

          <div className="HowItWorks__timeline">

            <div className="HowItWorks__timeline-row">
              <div className="HowItWorks__timeline-period">
                <div className="HowItWorks__timeline-title">Every month</div>
                <div className="HowItWorks__timeline-line HowItWorks__timeline-line-blue"></div>
              </div>
            </div>

            <div className="HowItWorks__timeline-row">
              <div className="HowItWorks__timeline-employees">
                <div className="HowItWorks__timeline-title">Employess</div>
                <div className="HowItWorks__timeline-line HowItWorks__timeline-line__arrow-right"></div>
                <div className="HowItWorks__timeline-employees__group">
                  <div className="HowItWorks__timeline-step">
                    <span className="number">1</span>
                    <span className="description">Exercise (a lot)</span>
                  </div>
                  <div className="HowItWorks__timeline-step">
                    <span className="number">2</span>
                    <span className="description">Collect points for kilometers or time</span>
                  </div>
                  <div className="HowItWorks__timeline-step">
                    <span className="number">3</span>
                    <span className="description">Nominate the beneficiary of the month</span>
                  </div>
                </div>
              </div>

              <div className="HowItWorks__timeline-employer">
                <div className="HowItWorks__timeline-title">Making Waves</div>
                <div className="HowItWorks__timeline-line HowItWorks__timeline-line__arrow-right"></div>
                <div className="HowItWorks__timeline-step">
                  <span className="number">4</span>
                  <span className="description">Exchange every 500 points to 100&nbsp;zł</span>
                </div>
              </div>

              <div className="HowItWorks__timeline-beneficiary">
                <div className="HowItWorks__timeline-title">Beneficiary</div>
                <div className="HowItWorks__timeline-line"></div>
                <div className="HowItWorks__timeline-step">
                  <span className="number">5</span>
                  <span className="description">Receives the money collected</span>
                </div>
              </div>
            </div>
          </div>

        </div>

      </div>
    );
  }
}

export default HowItWorks;

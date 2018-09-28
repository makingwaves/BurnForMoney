import React, { Component } from 'react';

import CharitySliderItem from './CharitySliderItem.js';
import './CharitySlider.css';


class CharitySlider extends Component {
  render() {
    return (
      <div className="CharitySlider">
        <div className="CurrentCharts__container container">
          <h2 className="CharitySlider__header Header"><strong>... for charity</strong></h2>
          <button href="#" className="Slider__prev"></button>
          <button href="#" className="Slider__next"></button>
          <div className="CharitySlider__container">
            <ul className="CharitySlider__list">
              <CharitySliderItem/>
              <li>
                Second option
              </li>
              <li>
                Third option
              </li>
            </ul>
          </div>
        </div>
      </div>
    );
  }
}

export default CharitySlider;

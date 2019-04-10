import React from "react";

const SVG = ({
  style = {},
  fill = "#9B9B9B",
  width="24px",
  height="24px",
  viewBox="0 0 24 24",
  className = ""
}) => (
  <svg
    width={width}
    style={style}
    height={width}
    viewBox={viewBox}
    xmlns="http://www.w3.org/2000/svg"
    className={`svg-icon ${className || ""}`}
    xmlnsXlink="http://www.w3.org/1999/xlink"
  >
  <g id="Symbols" stroke="none" strokeWidth="1" fill="none" fillRule="evenodd">
      <g id="Side-menu-/-dashboard" transform="translate(-20.000000, -257.000000)">
          <g id="Group-16" transform="translate(20.000000, 257.000000)">
              <g id="rank-army-star-badge-1">
                  <g id="Outline_Icons_1_" stroke={fill} strokeLinejoin="round">
                      <g id="Outline_Icons">
                          <g id="Group">
                              <polygon id="Shape" points="12.007 6.856125 13.5 10.28125 17 10.28125 14 12.7291667 15.5 16.6458333 12.007 14.053 8.5 16.6458333 10 12.7291667 7 10.28125 10.5 10.28125"></polygon>
                              <polygon id="Shape" points="21.284 13.5585208 23.5 11.75 21.284 9.94147917 22.625 7.4406875 19.871 6.6005625 20.132 3.78839583 17.259 4.04395833 16.4 1.34635417 13.846 2.65941667 12 0.489583333 10.153 2.65941667 7.6 1.34635417 6.741 4.04395833 3.868 3.78839583 4.129 6.6005625 1.375 7.4406875 2.716 9.94147917 0.5 11.75 2.716 13.5585208 1.375 16.0593125 4.129 16.8994375 3.868 19.7135625 6.741 19.4570208 7.6 22.1536458 10.154 20.8405833 12 23.0104167 13.847 20.8415625 16.401 22.154625 17.259 19.4570208 20.132 19.7135625 19.871 16.8994375 22.625 16.0593125"></polygon>
                          </g>
                      </g>
                  </g>
                  <g id="Invisible_Shape">
                      <rect id="Rectangle-path" x="0" y="0" width="24" height="23.5"></rect>
                  </g>
              </g>
          </g>
      </g>
  </g>
  </svg>
);

export default SVG;

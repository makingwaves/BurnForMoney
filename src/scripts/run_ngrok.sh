#!/bin/bash
../../addons/ngrok/ngrok http -host-header="localhost:$1" $1
import React, { Component } from 'react';
import axios from 'axios';
import Navbar from "./Navbar";
import '../styling/Prescriptions.css';
import '../styling/navbar.css';

export class Prescriptions extends Component {
    constructor(props) {
        super(props);

        this.state = {
            searchText: '',
            searchResults: [],
            prescriptions: [],
        };
    }

    componentDidMount() {
        // Check if user is logged in.
        var token = localStorage.getItem("token");
        if (token === "") {
            window.location.href = '/';
            return;
        }
    }

    handleSearchInputChange = (e) => {
        this.setState({
            searchText: e.target.value,
        });
    };

    handleSearchButtonClick = async () => {
        const { searchText } = this.state;

        try {
            const response = await axios.get(`/drug/${searchText}`, {
                headers: { "Authorization": localStorage.getItem('token') }
            });
            const searchResults = response.data;
            this.setState({
                searchResults,
            });
        } catch (error) {
            console.error('Error fetching search results:', error);
        }
    };

    handlePrescribeButtonClick = (drugId) => {

        const patientInfo = {
            patientId: 123,
            name: 'John Doe',
            socialNumber: '123-45-6789',
        };

        // Update prescriptions state with the new prescription
        this.setState(prevState => ({
            prescriptions: [...prevState.prescriptions, { drugId, ...patientInfo }],
        }));

    };

    render() {
        const { searchText, searchResults, prescriptions } = this.state;

        return (
            <>
                <Navbar />
                <div className="prescriptions-container">
                    <div className="search-box">
                        <input
                            type="text"
                            value={searchText}
                            onChange={this.handleSearchInputChange}
                            className="search-input"
                            placeholder="Find stoffer"
                        />
                        <button onClick={this.handleSearchButtonClick} className="search-button">
                            Find
                        </button>
                    </div>

                    <div className="search-results">
                        {searchResults.length > 0 && (
                            <ul className="results-list">
                                {searchResults.map((drug) => (
                                    <li key={drug.Id} className="result-item">
                                        <span className="result-name">{drug.Name}</span>
                                        <button
                                            onClick={() => this.handlePrescribeButtonClick(drug.Id)}
                                            className="prescribe-button">
                                            Skriv recept
                                        </button>
                                    </li>
                                ))}
                            </ul>
                        )}
                    </div>

                    {prescriptions.length > 0 && (
                        <div className="prescriptions-box">
                            <h2>Prescriptions</h2>
                            <div className="prescriptions-container">
                                {prescriptions.map((prescription, index) => (
                                    <div key={index} className="prescription-item">
                                        <div className="column">Patient ID: {prescription.patientId}</div>
                                        <div className="column">Navn: {prescription.name}</div>
                                        <div className="column">CPR: {prescription.socialNumber}</div>
                                        <button className="prescribe-button">
                                            Prescribe
                                        </button>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                </div>
            </>
        );
    }
}
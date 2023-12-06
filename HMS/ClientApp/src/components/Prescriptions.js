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
        };
    }

    handleSearchInputChange = (e) => {
        this.setState({
            searchText: e.target.value,
        });
    };

    handleSearchButtonClick = async () => {
        const { searchText } = this.state;

        try {
            const response = await axios.get(`/drug/${searchText}`,{
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
        // Add logic to handle prescription for the selected drug
        console.log('Prescribing drug with ID:', drugId);
    };

    render() {
        const { searchText, searchResults } = this.state;

        return (
            <>
            <Navbar/>
            <div className="prescriptions-container">
                <div>
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

                <div>
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
            </div>
            </>
        );
    }
}
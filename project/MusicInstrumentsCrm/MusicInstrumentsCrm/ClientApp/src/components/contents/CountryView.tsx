import React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import Country from "../../domain/Country";
import HttpMethod from "../../util/http/HttpMethods";
import { Strings } from '../../util/Strings';

export interface CountryViewProps extends RouteComponentProps {

}

export interface CountryViewState {
    countryList: Array<Country>;
}

export default class CountryView extends React.Component<CountryViewProps, CountryViewState> {

    constructor(props: CountryViewProps) {
        super(props, {});
        this.state = {
            countryList: new Array()
        };
    }

    componentDidMount() {
        let xhr = new XMLHttpRequest();
        xhr.open(HttpMethod.GET, 'http://localhost/api/v1/Country');
        xhr.onload = (evt) => {
            let res: Array<Country> = JSON.parse(xhr.responseText);
            this.setState({ countryList: res })
        };
        xhr.onerror = (evt) => {
            alert("error");
        };
        xhr.send();
    }
    handleRowClick(event: React.MouseEvent) {
        let id: string | null = null;
        if (event.currentTarget != null) {
            id = event.currentTarget.getAttribute('data-id');
        }
        if (!Strings.isNullOrEmpty(id)) {
            this.props.history.push(`/index/Country/edit/${id}`);
        }
    }

    public render() {
        return (
            <div className="content-view">
                <Link to="/index/CountryAdd"><button className="btn-content">Add</button></Link>
                
                <table className="table-content">
                    <tr>
                        <th>ID</th>
                        <th>Название</th>
                    </tr>
                    {
                        this.state.countryList.map((el: Country) => {
                            return <tr data-id={el.id} onClick={(evt) => { this.handleRowClick(evt); }}>
                                <td>{el.id}</td>
                                <td>{el.name}</td>
                            </tr>
                        })
                    }
                </table></div>
        );
    }
}
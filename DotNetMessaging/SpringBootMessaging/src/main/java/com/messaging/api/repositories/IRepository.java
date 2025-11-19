package com.messaging.api.repositories;

import com.messaging.api.models.BaseEntity;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.mongodb.core.aggregation.AggregationResults;

import java.util.List;
import java.util.Optional;

public interface IRepository<T extends BaseEntity> {
    T save(T entity);
    
    Optional<T> findById(String id);
    
    List<T> findAll();
    
    Page<T> findAll(Pageable pageable);
    
    List<T> findByField(String fieldName, Object value);
    
    void deleteById(String id);
    
    void delete(T entity);
    
    long count();
    
    boolean existsById(String id);
    
    // Aggregation support
    <O> AggregationResults<O> aggregate(org.springframework.data.mongodb.core.aggregation.Aggregation aggregation, Class<O> outputType);
}

